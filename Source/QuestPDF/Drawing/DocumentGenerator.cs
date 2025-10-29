using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Companion;
using QuestPDF.Drawing.DocumentCanvases;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using PDFA_Conformance = QuestPDF.Infrastructure.PDFA_Conformance;
using PDFUA_Conformance = QuestPDF.Infrastructure.PDFUA_Conformance;

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

            using var canvas = new PdfDocumentCanvas(stream, metadata, settings);
            RenderDocument(canvas, document, settings);
        }
        
        internal static void GenerateXps(SkWriteStream stream, IDocument document)
        {
            ValidateLicense();
            
            var settings = document.GetSettings();
            using var canvas = new XpsDocumentCanvas(stream, settings);
            RenderDocument(canvas, document, settings);
        }
        
        internal static ICollection<byte[]> GenerateImages(IDocument document, ImageGenerationSettings imageGenerationSettings)
        {
            ValidateLicense();
            
            var documentSettings = document.GetSettings();
            documentSettings.ImageRasterDpi = imageGenerationSettings.RasterDpi;
            
            using var canvas = new ImageDocumentCanvas(imageGenerationSettings);
            RenderDocument(canvas, document, documentSettings);

            return canvas.Images;
        }
        
        internal static ICollection<string> GenerateSvg(IDocument document)
        {
            ValidateLicense();
            
            using var canvas = new SvgDocumentCanvas();
            RenderDocument(canvas, document, document.GetSettings());

            return canvas.Images;
        }
        
        internal static void GenerateAndDiscard(IDocument document)
        {
            RenderDocument(new FreeDocumentCanvas(), document, DocumentSettings.Default);
        }

        internal static void ValidateLicense()
        {
            if (Settings.License.HasValue)
                return;
            
            const string newParagraph = "\n\n";

            var exceptionMessage = 
                $"{newParagraph}{newParagraph}Welcome to QuestPDF! 👋 {newParagraph}" +
                $"QuestPDF is an open-source library committed to long-term sustainability and continuous improvement. {newParagraph}" +
                $"To maintain high-quality development and support while keeping the library free for most users, we use a fair pricing model where only larger organizations help by providing necessary funding for the project. {newParagraph}" +
                $"If your organization’s annual gross revenue exceeds $1M USD, a Commercial License is required for production use (you can freely evaluate and test QuestPDF in non-production environments at no cost). " +
                $"In that case, please share this information with your team or management. By purchasing a license, you directly contribute to the ongoing development and reliability of QuestPDF. {newParagraph}" +
                $"For details on the license types and determining which applies to you, please visit: https://www.questpdf.com/license/ {newParagraph}" +
                $"We trust our users. To continue, no license key is needed. Instead, simply select and configure the appropriate license in your code. For example, if you qualify for the Community license, add: {newParagraph}" +
                $"> QuestPDF.Settings.License = LicenseType.Community; {newParagraph}" +
                $"Thank you for supporting QuestPDF! ❤️ By choosing the right license, you help ensure that our project remains transparent, sustainable, and continuously improving for everyone. 🚀 {newParagraph}{newParagraph}";
            
            throw new Exception(exceptionMessage)
            {
                HelpLink = "https://www.questpdf.com/pricing.html"
            };
        }

        internal static CompanionDocumentSnapshot GenerateCompanionContent(IDocument document)
        {
            using var canvas = new CompanionDocumentCanvas();
            RenderDocument(canvas, document, DocumentSettings.Default);
            return canvas.GetContent();
        }

        internal static void RenderDocument(IDocumentCanvas canvas, IDocument document, DocumentSettings settings)
        {
            if (document is MergedDocument mergedDocument)
            {
                RenderMergedDocument(canvas, mergedDocument, settings);
                return;
            }
            
            // TODO: handle Header nesting values
            var semanticTreeManager = CreateSemanticTreeManager(settings);
            var useOriginalImages = canvas is ImageDocumentCanvas;
            var content = ConfigureContent(document, settings, semanticTreeManager, useOriginalImages);
            
            if (canvas is CompanionDocumentCanvas)
                content.VisitChildren(x => x.CreateProxy(y => new LayoutProxy(y)));
            
            try
            {
                var pageContext = new PageContext();
                RenderPass(pageContext, new FreeDocumentCanvas(), content);
                pageContext.ProceedToNextRenderingPhase();

                canvas.ConfigureWithSemanticTree(semanticTreeManager);
                
                canvas.BeginDocument();
                RenderPass(pageContext, canvas, content);
                canvas.EndDocument();
            
                if (canvas is CompanionDocumentCanvas companionCanvas)
                    companionCanvas.Hierarchy = content.ExtractHierarchy();
            }
            finally
            {
                content.ReleaseDisposableChildren();
            }
        }
        
        private static void RenderMergedDocument(IDocumentCanvas canvas, MergedDocument document, DocumentSettings settings)
        {
            var useOriginalImages = canvas is ImageDocumentCanvas;
            
            var sharedPageContent = new PageContext();
            var useSharedPageContext = document.PageNumberStrategy == MergedDocumentPageNumberStrategy.Continuous;

            var semanticTreeManager = CreateSemanticTreeManager(settings);
            
            var documentParts = Enumerable
                .Range(0, document.Documents.Count)
                .Select(index => new
                {
                    DocumentId = index,
                    Content = ConfigureContent(document.Documents[index], settings, semanticTreeManager, useOriginalImages),
                    PageContext = useSharedPageContext ? sharedPageContent : new PageContext()
                })
                .ToList();
            
            try
            {
                foreach (var documentPart in documentParts)
                    documentPart.PageContext.SetDocumentId(documentPart.DocumentId);
                
                foreach (var documentPart in documentParts)
                {
                    RenderPass(documentPart.PageContext, new FreeDocumentCanvas(), documentPart.Content);
                    documentPart.PageContext.ProceedToNextRenderingPhase();
                }

                canvas.ConfigureWithSemanticTree(semanticTreeManager);
                
                canvas.BeginDocument();

                foreach (var documentPart in documentParts)
                {
                    RenderPass(documentPart.PageContext, canvas, documentPart.Content);
                    documentPart.Content.ReleaseDisposableChildren();
                }
                
                canvas.EndDocument();
            }
            finally
            {
                documentParts.ForEach(x => x.Content.ReleaseDisposableChildren());
            }
        }

        private static SemanticTreeManager? CreateSemanticTreeManager(DocumentSettings settings)
        {
            return IsDocumentSemanticAware() ? new SemanticTreeManager() : null;

            bool IsDocumentSemanticAware()
            {
                if (settings.PDFUA_Conformance is not PDFUA_Conformance.None)
                    return true;
                
                if (settings.PDFA_Conformance is PDFA_Conformance.PDFA_1A or PDFA_Conformance.PDFA_2A or PDFA_Conformance.PDFA_3A)
                    return true;

                return false;
            }
        }

        private static void ConfigureWithSemanticTree(this IDocumentCanvas canvas, SemanticTreeManager? semanticTreeManager)
        {
            if (semanticTreeManager == null) 
                return;
            
            var semanticTree = semanticTreeManager.GetSemanticTree();
            semanticTreeManager.Reset();
            canvas.SetSemanticTree(semanticTree);
        }

        private static Container ConfigureContent(IDocument document, DocumentSettings settings, SemanticTreeManager? semanticTreeManager, bool useOriginalImages)
        {
            var container = new DocumentContainer();
            document.Compose(container);
            
            var content = container.Compose();
            
            content.ApplyInheritedAndGlobalTexStyle(TextStyle.Default);
            content.ApplyContentDirection(settings.ContentDirection);
            content.ApplyDefaultImageConfiguration(settings.ImageRasterDpi, settings.ImageCompressionQuality, useOriginalImages);

            if (Settings.EnableCaching)
                content.ApplyCaching();
            
            if (semanticTreeManager != null)
            {
                content.ApplySemanticParagraphs();
                content.InjectSemanticTreeManager(semanticTreeManager);
            }
            
            return content;
        }

        private static void RenderPass(PageContext pageContext, IDocumentCanvas canvas, ContainerElement content)
        {
            content.InjectDependencies(pageContext, canvas.GetDrawingCanvas());
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
                content.VisitChildren(x => (x as SnapshotCacheRecorderProxy)?.Dispose());
                content.RemoveExistingProxiesOfType<SnapshotCacheRecorderProxy>();

                content.ApplyLayoutOverflowDetection();
                content.Measure(Size.Max);

                var overflowState = content.ExtractElementsOfType<OverflowDebuggingProxy>().Single();
                overflowState.StopMeasuring();
                overflowState.TryToFixTheLayoutOverflowIssue();
                
                content.ApplyContentDirection();
                content.InjectDependencies(pageContext, canvas.GetDrawingCanvas());

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
                }
                else
                {
                    message +=
                        $"To further investigate the location of the root cause, please run the application with a debugger attached or set the {debuggingSettingsName} flag to true. " +
                        $"The library will generate additional debugging information such as probable code problem location and detailed layout measurement overview.";
                }
                
                throw new DocumentLayoutException(message);
            }
            
            (ICollection<Element> ancestors, TreeNode<OverflowDebuggingProxy> layout) GenerateLayoutExceptionDebuggingInfo()
            {
                content.RemoveExistingProxies();
                content.ApplyLayoutOverflowDetection();
                content.Measure(Size.Max);
                
                var overflowState = content.ExtractElementsOfType<OverflowDebuggingProxy>().Single();
                overflowState.StopMeasuring();
                overflowState.TryToFixTheLayoutOverflowIssue();

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

        internal static void InjectSemanticTreeManager(this Element content, SemanticTreeManager semanticTreeManager)
        {
            content.VisitChildren(x =>
            {
                if (x is ISemanticAware semanticAware)
                {
                    semanticAware.SemanticTreeManager = semanticTreeManager;
                }
                else if (x is TextBlock textBlock)
                {
                    foreach (var textBlockElement in textBlock.Items.OfType<TextBlockElement>())
                    {
                        textBlockElement.Element.InjectSemanticTreeManager(semanticTreeManager);
                    }
                }
            });
        }
        
        internal static void InjectDependencies(this Element content, IPageContext pageContext, IDrawingCanvas canvas)
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
                content?.CreateProxy(x => new SnapshotCacheRecorderProxy(x));

            // returns true if can apply caching
            bool Traverse(Element? content)
            {
                if (content is TextBlock textBlock)
                {
                    foreach (var textBlockItem in textBlock.Items)
                    {
                        if (textBlockItem is TextBlockPageNumber)
                            return false;
                        
                        if (textBlockItem is TextBlockElement textBlockElement && !Traverse(textBlockElement.Element))
                            return false;
                    }

                    return true;
                }

                if (content is Lazy lazy)
                    return lazy.IsCacheable;

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

                    return canApplyCaching ? new SnapshotCacheRecorderProxy(x) : x;
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
                
                if (x is Lazy lazy)
                {
                    lazy.ImageTargetDpi ??= imageRasterDpi;
                    lazy.ImageCompressionQuality ??= imageCompressionQuality;
                    lazy.UseOriginalImage |= useOriginalImages;
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
            
            if (content is Lazy lazy)
                lazy.TextStyle = lazy.TextStyle.ApplyInheritedStyle(documentDefaultTextStyle);
            
            if (content is DefaultTextStyle defaultTextStyleElement)
               documentDefaultTextStyle = defaultTextStyleElement.TextStyle.ApplyInheritedStyle(documentDefaultTextStyle);

            if (content is ContainerElement containerElement)
            {
                ApplyInheritedAndGlobalTexStyle(containerElement.Child, documentDefaultTextStyle);
            }
            else
            {
                foreach (var child in content.GetChildren())
                    ApplyInheritedAndGlobalTexStyle(child, documentDefaultTextStyle);
            }
        }

        internal static void ApplySemanticParagraphs(this Element root)
        {
            var textContextLevel = 0;
            var isFooterContext = false;
            
            Traverse(root);
            
            void Traverse(Element element)
            {
                if (element is SemanticTag semanticTag)
                {
                    var isTextSemanticTag = semanticTag.TagType is "H" or "H1" or "H2" or "H3" or "H4" or "H5" or "H6" or "P";

                    if (isTextSemanticTag)
                        textContextLevel++;
                    
                    Traverse(semanticTag.Child);
                    
                    if (isTextSemanticTag)
                        textContextLevel--;
                }
                else if (element is ArtifactTag)
                {
                    // ignore all Text elements that are marked as artifacts
                }
                else if (element is DebugPointer { Type: DebugPointerType.DocumentStructure, Label: nameof(DocumentStructureTypes.Footer) } debugPointer)
                {
                    isFooterContext = true;
                    Traverse(debugPointer.Child);
                    isFooterContext = false;
                }
                else if (element is ContainerElement container)
                {
                    if (container.Child is TextBlock textBlock)
                    {
                        if (textContextLevel > 0)
                            return;

                        var textBlockContainsPageNumber = textBlock.Items.Any(x => x is TextBlockPageNumber);
                        
                        if (isFooterContext && textBlockContainsPageNumber)
                            return;
                        
                        container.CreateProxy(x => new SemanticTag
                        {
                            Child = x,
                            TagType = "P"
                        });
                    }
                    else
                    {
                        Traverse(container.Child);
                    }
                }
                else
                {
                    foreach (var child in element.GetChildren())
                        Traverse(child);
                }
            }
        }
    }
}
