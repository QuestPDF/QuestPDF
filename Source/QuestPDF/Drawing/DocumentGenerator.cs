using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            NativeDependencyCompatibilityChecker.Test();
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

        private static void ValidateLicense()
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

        internal static PreviewerDocumentSnapshot GeneratePreviewerContent(IDocument document)
        {
            var canvas = new PreviewerCanvas();
            RenderDocument(canvas, document, DocumentSettings.Default);
            return canvas.GetContent();
        }
        
        private static void RenderDocument<TCanvas>(TCanvas canvas, IDocument document, DocumentSettings settings) where TCanvas : ICanvas, IRenderingCanvas
        {
            canvas.BeginDocument();
            
            if (document is MergedDocument mergedDocument)
                RenderMergedDocument(canvas, mergedDocument, settings);
            
            else
                RenderSingleDocument(canvas, document, settings);
            
            canvas.EndDocument();
        }

        private static void RenderSingleDocument<TCanvas>(TCanvas canvas, IDocument document, DocumentSettings settings)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            var useOriginalImages = canvas is ImageCanvas;

            var content = ConfigureContent(document, settings, useOriginalImages);

            var pageContext = new PageContext();
            RenderPass(pageContext, new FreeCanvas(), content);
            pageContext.ProceedToNextRenderingPhase();
            RenderPass(pageContext, canvas, content);
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
            
            return content;
        }

        private static void RenderPass<TCanvas>(PageContext pageContext, TCanvas canvas, ContainerElement content)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            content.InjectDependencies(pageContext, canvas);
            content.VisitChildren(x => (x as IStateResettable)?.ResetState(hardReset: true));

            while(true)
            {
                pageContext.IncrementPageNumber();
                var spacePlan = content.Measure(Size.Max);

                if (spacePlan.Type == SpacePlanType.Wrap)
                {
                    pageContext.DecrementPageNumber();
                    
                    if (Settings.EnableDebugging)
                    {
                        ApplyLayoutDebugging();
                        continue;
                    }
                    
                    canvas.EndDocument();
                    ThrowLayoutException();
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
                content.RemoveExistingProxies();

                content.ApplyLayoutOverflowDetection();
                content.Measure(Size.Max);

                var overflowState = content.ExtractElementsOfType<OverflowDebuggingProxy>().Single();
                overflowState.StopMeasuring();
                overflowState.ApplyLayoutOverflowVisualization();
                
                content.ApplyContentDirection();
                content.InjectDependencies(pageContext, canvas);

                content.RemoveExistingProxies();
            }
            
            void ThrowLayoutException()
            {
                content.RemoveExistingProxies();
                content.ApplyLayoutOverflowDetection();
                content.Measure(Size.Max);
                
                var overflowState = content.ExtractElementsOfType<OverflowDebuggingProxy>().Single();
                overflowState.StopMeasuring();
                overflowState.ApplyLayoutOverflowVisualization();

                var rootCause = overflowState.FindLayoutOverflowVisualizationNodes().First();
                
                var stack = rootCause
                    .ExtractAncestors()
                    .Select(x => x.Value)
                    .Reverse()
                    .FormatAncestors();

                var inside = rootCause
                    .ExtractAncestors()
                    .First(x => x.Value.Child is SourceCodePointer or DebugPointer)
                    .Children
                    .First()
                    .FormatLayoutSubtree();
                
                var newLine = "\n";
                var newParagraph = newLine + newLine;
                    
                var message =
                    $"The provided document content contains conflicting size constraints. " +
                    $"For example, some elements may require more space than is available. {newParagraph}" +
                    $"To quickly determine the place where the problem is likely occurring, please generate the document with the attached debugger. " +
                    $"The library will generate a PDF document with visually annotated places where layout constraints are invalid. {newParagraph}" +
                    $"Alternatively, if you don’t want to or cannot attach the debugger, you can set the {nameof(QuestPDF)}.{nameof(Settings)}.{nameof(Settings.EnableDebugging)} flag to true. {newParagraph}" +
                    $"The layout issue is likely present in the following part of the document: {newParagraph}{stack}{newParagraph}" +
                    $"Please analyse the document measurement to learn more: {newParagraph}{inside}" +
                    $"{LayoutDebugging.LayoutVisualizationLegend}";
                
                throw new DocumentLayoutException(message);
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
