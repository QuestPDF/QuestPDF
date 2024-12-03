using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using QuestPDF.Companion;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing
{
    static class DocumentGenerator
    {
        static DocumentGenerator()
        {
            SkNativeDependencyCompatibilityChecker.Test();
        }
        
        internal static void GeneratePdf(SkWriteStream stream, IDocument document)
        {
            ValidateLicense();
            
            var metadata = document.GetMetadata();
            var settings = document.GetSettings();
            var canvas = new PdfCanvas(stream, metadata, settings);
            RenderDocument(canvas, document, settings);
        }
        
        internal static void GenerateXps(SkWriteStream stream, IDocument document)
        {
            ValidateLicense();
            
            var settings = document.GetSettings();
            var canvas = new XpsCanvas(stream, settings);
            RenderDocument(canvas, document, settings);
        }
        
        internal static ICollection<byte[]> GenerateImages(IDocument document, ImageGenerationSettings imageGenerationSettings)
        {
            ValidateLicense();
            
            var documentSettings = document.GetSettings();
            documentSettings.ImageRasterDpi = imageGenerationSettings.RasterDpi;
            
            var canvas = new ImageCanvas(imageGenerationSettings);
            RenderDocument(canvas, document, documentSettings);

            return canvas.Images;
        }
        
        internal static ICollection<string> GenerateSvg(IDocument document)
        {
            ValidateLicense();
            
            var canvas = new SvgCanvas();
            RenderDocument(canvas, document, document.GetSettings());

            return canvas.Images;
        }

        internal static void ValidateLicense()
        {
            if (Settings.License.HasValue)
                return;
            
            const string newParagraph = "\n\n";

            var exceptionMessage = 
                $"QuestPDF is a modern open-source library. " +
                $"We identify the importance of the library in your projects and therefore want to make sure you can safely and confidently continue the development. " +
                $"Being a healthy and growing community is the primary goal that motivates us to pursue professionalism. {newParagraph}" +
                $"Please refer to the QuestPDF License and Pricing webpage for more details. (https://www.questpdf.com/license/) {newParagraph}" +
                $"If you are an existing QuestPDF user and for any reason cannot update, you can stay with the 2022.12.X release with the extended quality support but without any new features, improvements, or optimizations. " +
                $"That release will always be available under the MIT license, free for commercial usage. We are planning to sunset support for the 2022.12.X branch around Q1 2024. Until then, it will continue to receive quality and bug-fix updates. {newParagraph}" +
                $"The library does not require any license key. We trust our users, and therefore the process is simple. " +
                $"To disable license validation and turn off this exception, please configure an eligible license using the QuestPDF.Settings.License API, for example: {newParagraph}" +
                $"\"QuestPDF.Settings.License = LicenseType.Community;\" {newParagraph}" +
                $"Learn more on: https://www.questpdf.com/license/configuration.html {newParagraph}";
            
            throw new Exception(exceptionMessage)
            {
                HelpLink = "https://www.questpdf.com/pricing.html"
            };
        }

        internal static CompanionDocumentSnapshot GenerateCompanionContent(IDocument document)
        {
            var canvas = new CompanionCanvas();
            RenderDocument(canvas, document, DocumentSettings.Default);
            return canvas.GetContent();
        }

        /// <summary>
        /// Adjusts the concurrency level for the RenderDocument method to optimize performance.
        /// 
        /// While the Skia implementation is thread-safe, it appears to contain internal locks that hinder scalability.
        /// Consequently, using multithreading for document rendering can reduce performance. This includes increased memory usage 
        /// and slower generation times—potentially even worse than rendering documents sequentially.
        /// 
        /// Based on testing, using two concurrent threads yields the best performance in the current environment, 
        /// though it is still not optimal. A potential area for investigation is the `SkParagraph.PlanLayout` method, 
        /// which utilizes a paragraph cache within the `SkFontCollection` class. 
        /// 
        /// Notably:
        /// - In theory, the paragraph cache is already disabled.
        /// - Despite this, the method does not scale linearly with the number of CPU cores, indicating underlying synchronization bottlenecks.
        /// 
        /// Other potential contributors to the scalability issues may include:
        /// 1. Garbage Collection: The generation process might create a large number of layout element instances 
        ///    whose lifetimes span the entire document rendering process.
        /// 2. CPU Cache Efficiency: Complex, large, and deeply nested document layout trees may lead to cache misses 
        ///    as they are repeatedly traversed during rendering.
        /// 
        /// TODO:
        /// - Investigate the underlying causes of these scalability issues in future releases.
        /// - Explore optimizations for reducing locking contention, improving memory efficiency, and enhancing CPU cache utilization.
        /// </summary>
        private static SemaphoreSlim RenderDocumentSemaphore = new(2);
        
        private static void RenderDocument<TCanvas>(TCanvas canvas, IDocument document, DocumentSettings settings) where TCanvas : ICanvas, IRenderingCanvas
        {
            RenderDocumentSemaphore.Wait();

            try
            {
                canvas.BeginDocument();
            
                if (document is MergedDocument mergedDocument)
                    RenderMergedDocument(canvas, mergedDocument, settings);
            
                else
                    RenderSingleDocument(canvas, document, settings);
            
                canvas.EndDocument();
            }
            finally
            {
                RenderDocumentSemaphore.Release();
            }
        }

        private static void RenderSingleDocument<TCanvas>(TCanvas canvas, IDocument document, DocumentSettings settings)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            var useOriginalImages = canvas is ImageCanvas;

            var content = ConfigureContent(document, settings, useOriginalImages);
            
            if (canvas is CompanionCanvas)
                content.VisitChildren(x => x.CreateProxy(y => new LayoutProxy(y)));
            
            var pageContext = new PageContext();
            RenderPass(pageContext, new FreeCanvas(), content);
            pageContext.ProceedToNextRenderingPhase();
            RenderPass(pageContext, canvas, content);
            
            if (canvas is CompanionCanvas companionCanvas)
                companionCanvas.Hierarchy = content.ExtractHierarchy();
        }
        
        private static void RenderMergedDocument<TCanvas>(TCanvas canvas, MergedDocument document, DocumentSettings settings)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            var useOriginalImages = canvas is ImageCanvas;
            
            var documentParts = Enumerable
                .Range(0, document.Documents.Count)
                .Select(index => new
                {
                    DocumentId = index,
                    Content = ConfigureContent(document.Documents[index], settings, useOriginalImages)
                })
                .ToList();

            if (document.PageNumberStrategy == MergedDocumentPageNumberStrategy.Continuous)
            {
                var documentPageContext = new PageContext();

                foreach (var documentPart in documentParts)
                {
                    documentPageContext.SetDocumentId(documentPart.DocumentId);
                    RenderPass(documentPageContext, new FreeCanvas(), documentPart.Content);
                }
                
                documentPageContext.ProceedToNextRenderingPhase();

                foreach (var documentPart in documentParts)
                {
                    documentPageContext.SetDocumentId(documentPart.DocumentId);
                    RenderPass(documentPageContext, canvas, documentPart.Content);   
                }
            }
            else
            {
                foreach (var documentPart in documentParts)
                {
                    var pageContext = new PageContext();
                    pageContext.SetDocumentId(documentPart.DocumentId);
                    
                    RenderPass(pageContext, new FreeCanvas(), documentPart.Content);
                    pageContext.ProceedToNextRenderingPhase();
                    RenderPass(pageContext, canvas, documentPart.Content);
                }
            }
        }

        private static Container ConfigureContent(IDocument document, DocumentSettings settings, bool useOriginalImages)
        {
            var container = new DocumentContainer();
            document.Compose(container);
            
            var content = container.Compose();
            
            content.ApplyInheritedAndGlobalTexStyle(TextStyle.Default);
            content.ApplyContentDirection(settings.ContentDirection);
            content.ApplyDefaultImageConfiguration(settings.ImageRasterDpi, settings.ImageCompressionQuality, useOriginalImages);

            if (Settings.EnableCaching)
                content.ApplyCaching();
            
            return content;
        }

        private static void RenderPass<TCanvas>(PageContext pageContext, TCanvas canvas, ContainerElement content)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            content.InjectDependencies(pageContext, canvas);
            content.VisitChildren(x => (x as IStateful)?.ResetState(hardReset: true));

            while(true)
            {
                pageContext.IncrementPageNumber();
                var spacePlan = content.Measure(Size.Max);

                if (spacePlan.Type == SpacePlanType.Wrap)
                {
                    pageContext.DecrementPageNumber();
                    canvas.EndDocument();

                    #if NET6_0_OR_GREATER
                    if (!CompanionService.IsCompanionAttached)
                        ThrowLayoutException();
                    #else
                    ThrowLayoutException();
                    #endif
                    
                    ApplyLayoutDebugging();
                }

                try
                {
                    canvas.BeginPage(spacePlan);
                    content.Draw(spacePlan);
                }
                catch (Exception exception)
                {
                    canvas.EndDocument();
                    throw new DocumentDrawingException("An exception occured during document drawing.", exception);
                }

                canvas.EndPage();

                if (spacePlan.Type == SpacePlanType.FullRender)
                    break;
            }
            
            void ApplyLayoutDebugging()
            {
                content.RemoveExistingProxiesOfType<SnapshotRecorder>();

                content.ApplyLayoutOverflowDetection();
                content.Measure(Size.Max);

                var overflowState = content.ExtractElementsOfType<OverflowDebuggingProxy>().Single();
                overflowState.StopMeasuring();
                overflowState.ApplyLayoutOverflowVisualization();
                
                content.ApplyContentDirection();
                content.InjectDependencies(pageContext, canvas);

                content.VisitChildren(x => (x as LayoutProxy)?.CaptureLayoutErrorMeasurement());
                content.RemoveExistingProxiesOfType<OverflowDebuggingProxy>();
            }
            
            void ThrowLayoutException()
            {
                var newLine = "\n";
                var newParagraph = newLine + newLine;
                
                const string debuggingSettingsName = $"{nameof(QuestPDF)}.{nameof(Settings)}.{nameof(Settings.EnableDebugging)}";

                var message =
                    $"The provided document content contains conflicting size constraints. " +
                    $"For example, some elements may require more space than is available. {newParagraph}";
                
                if (Settings.EnableDebugging)
                {
                    var (ancestors, layout) = GenerateLayoutExceptionDebuggingInfo();

                    var ancestorsText = ancestors.FormatAncestors();
                    var layoutText = layout.FormatLayoutSubtree();

                    message +=
                        $"The layout issue is likely present in the following part of the document: {newParagraph}{ancestorsText}{newParagraph}" +
                        $"To learn more, please analyse the document measurement of the problematic location: {newParagraph}{layoutText}" +
                        $"{LayoutDebugging.LayoutVisualizationLegend}{newParagraph}" +
                        $"This detailed information is generated because you run the application with a debugger attached or with the {debuggingSettingsName} flag set to true. ";

                    throw new DocumentLayoutException(message);
                }
                else
                {
                    message +=
                        $"To further investigate the location of the root cause, please run the application with a debugger attached or set the {debuggingSettingsName} flag to true. " +
                        $"The library will generate additional debugging information such as probable code problem location and detailed layout measurement overview.";
                    
                    throw new DocumentLayoutException(message);
                }
            }
            
            (ICollection<Element> ancestors, TreeNode<OverflowDebuggingProxy> layout) GenerateLayoutExceptionDebuggingInfo()
            {
                content.RemoveExistingProxies();
                content.ApplyLayoutOverflowDetection();
                content.Measure(Size.Max);
                
                var overflowState = content.ExtractElementsOfType<OverflowDebuggingProxy>().Single();
                overflowState.StopMeasuring();
                overflowState.ApplyLayoutOverflowVisualization();

                var rootCause = overflowState.FindLayoutOverflowVisualizationNodes().First();
                
                var ancestors = rootCause
                    .ExtractAncestors()
                    .Select(x => x.Value.Child)
                    .Where(x => x is DebugPointer or SourceCodePointer)
                    .Reverse()
                    .ToArray();

                var layout = rootCause
                    .ExtractAncestors()
                    .First(x => x.Value.Child is SourceCodePointer or DebugPointer)
                    .Children
                    .First();

                return (ancestors, layout);
            }
        }

        internal static void InjectDependencies(this Element content, IPageContext pageContext, ICanvas canvas)
        {
            content.VisitChildren(x =>
            {
                if (x == null)
                    return;
                
                x.PageContext = pageContext;
                x.Canvas = canvas;
            });
        }
        
        internal static void ApplyCaching(this Element? content)
        {
            var canApplyCaching = Traverse(content);
            
            if (canApplyCaching)
                content?.CreateProxy(x => new SnapshotRecorder(x));

            // returns true if can apply caching
            bool Traverse(Element? content)
            {
                if (content is TextBlock textBlock)
                {
                    foreach (var textBlockItem in textBlock.Items)
                    {
                        if (textBlockItem is TextBlockPageNumber)
                            return false;
                        
                        if (textBlockItem is TextBlockElement textBlockElement)
                            return Traverse(textBlockElement.Element);
                    }

                    return true;
                }

                if (content is DynamicHost)
                    return false;
                
                if (content is ContainerElement containerElement)
                    return Traverse(containerElement.Child);

                if (content is MultiColumn multiColumn)
                {
                    var multiColumnSupportsCaching = Traverse(multiColumn.Content) && Traverse(multiColumn.Spacer);
                    
                    multiColumn.Content.RemoveExistingProxies();
                    multiColumn.Spacer.RemoveExistingProxies();
                    
                    return multiColumnSupportsCaching;
                }

                var canApplyCachingPerChild = content.GetChildren().Select(Traverse).ToArray();
                
                if (canApplyCachingPerChild.All(x => x))
                    return true;

                if (content is Row row && row.Items.Any(x => x.Type == RowItemType.Auto))
                    return false;

                var childIndex = 0;
                
                content.CreateProxy(x =>
                {
                    var canApplyCaching = canApplyCachingPerChild[childIndex];
                    childIndex++;

                    return canApplyCaching ? new SnapshotRecorder(x) : x;
                });
                
                return false;
            }
        }
        
        internal static void ApplyContentDirection(this Element? content, ContentDirection? direction = null)
        {
            if (content == null)
                return;

            if (content is ContentDirectionSetter contentDirectionSetter)
            {
                ApplyContentDirection(contentDirectionSetter.Child, contentDirectionSetter.ContentDirection);
                return;
            }

            if (content is IContentDirectionAware contentDirectionAware)
                contentDirectionAware.ContentDirection = direction ?? contentDirectionAware.ContentDirection;
            
            foreach (var child in content.GetChildren())
                ApplyContentDirection(child, direction);
        }
        
        internal static void ApplyDefaultImageConfiguration(this Element? content, int imageRasterDpi, ImageCompressionQuality imageCompressionQuality, bool useOriginalImages)
        {
            content.VisitChildren(x =>
            {
                if (x is QuestPDF.Elements.Image image)
                {
                    image.TargetDpi ??= imageRasterDpi;
                    image.CompressionQuality ??= imageCompressionQuality;
                    image.UseOriginalImage |= useOriginalImages;
                }

                if (x is QuestPDF.Elements.DynamicImage dynamicImage)
                {
                    dynamicImage.TargetDpi ??= imageRasterDpi;
                    dynamicImage.CompressionQuality ??= imageCompressionQuality;
                    dynamicImage.UseOriginalImage |= useOriginalImages;
                }

                if (x is DynamicHost dynamicHost)
                {
                    dynamicHost.ImageTargetDpi ??= imageRasterDpi;
                    dynamicHost.ImageCompressionQuality ??= imageCompressionQuality;
                    dynamicHost.UseOriginalImage |= useOriginalImages;
                }

                if (x is TextBlock textBlock)
                {
                    foreach (var textBlockElement in textBlock.Items.OfType<TextBlockElement>())
                    {
                        textBlockElement.Element.ApplyDefaultImageConfiguration(imageRasterDpi, imageCompressionQuality, useOriginalImages);
                    }
                }
            });
        }

        internal static void ApplyInheritedAndGlobalTexStyle(this Element? content, TextStyle documentDefaultTextStyle)
        {
            if (content == null)
                return;
            
            if (content is TextBlock textBlock)
            {
                textBlock.DefaultTextStyle = textBlock.DefaultTextStyle.ApplyInheritedStyle(documentDefaultTextStyle).ApplyGlobalStyle();
                
                foreach (var textBlockItem in textBlock.Items)
                {
                    if (textBlockItem is TextBlockSpan textSpan)
                        textSpan.Style = textSpan.Style.ApplyInheritedStyle(documentDefaultTextStyle).ApplyGlobalStyle();
                    
                    if (textBlockItem is TextBlockElement textElement)
                        ApplyInheritedAndGlobalTexStyle(textElement.Element, documentDefaultTextStyle);
                }
                
                return;
            }

            if (content is DynamicHost dynamicHost)
                dynamicHost.TextStyle = dynamicHost.TextStyle.ApplyInheritedStyle(documentDefaultTextStyle);
            
            if (content is DefaultTextStyle defaultTextStyleElement)
               documentDefaultTextStyle = defaultTextStyleElement.TextStyle.ApplyInheritedStyle(documentDefaultTextStyle);

            foreach (var child in content.GetChildren())
                ApplyInheritedAndGlobalTexStyle(child, documentDefaultTextStyle);
        }
    }
}
