using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing
{
    static class DocumentGenerator
    {
        internal static void GeneratePdf(Stream stream, IDocument document)
        {
            ValidateLicense();
            CheckIfStreamIsCompatible(stream);
            
            var metadata = document.GetMetadata();
            var settings = document.GetSettings();
            var canvas = new PdfCanvas(stream, metadata, settings);
            RenderDocument(canvas, document, settings);
        }
        
        internal static void GenerateXps(Stream stream, IDocument document)
        {
            ValidateLicense();
            CheckIfStreamIsCompatible(stream);
            
            var settings = document.GetSettings();
            var canvas = new XpsCanvas(stream, settings);
            RenderDocument(canvas, document, settings);
        }

        private static void CheckIfStreamIsCompatible(Stream stream)
        {
            if (!stream.CanWrite)
                throw new ArgumentException("The library requires a Stream object with the 'write' capability available (the CanWrite flag). Please consider using the MemoryStream class.");
            
            if (!stream.CanSeek)
                throw new ArgumentException("The library requires a Stream object with the 'seek' capability available (the CanSeek flag). Please consider using the MemoryStream class.");
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

        private static void ValidateLicense()
        {
            if (Settings.License.HasValue)
                return;
            
            var newParagraph = Environment.NewLine + Environment.NewLine;

            var exceptionMessage = 
                $"QuestPDF is a modern open-source library. " +
                $"We identify the importance of the library in your projects and therefore want to make sure you can safely and confidently continue the development. " +
                $"Being a healthy and growing community is the primary goal that motivates us to pursue professionalism. {newParagraph}" +
                $"Please refer to the QuestPDF License and Pricing webpage for more details. (https://www.questpdf.com/pricing.html) {newParagraph}" +
                $"If you are an existing QuestPDF user and for any reason cannot update, you can stay with the 2022.12.X release with the extended quality support but without any new features, improvements, or optimizations. That release will always be available under the MIT license, free for commercial usage. {newParagraph}" +
                $"The library does not require any license key. We trust our users, and therefore the process is simple. " +
                $"To disable license validation and turn off this exception, please configure an eligible license using the QuestPDF.Settings.License API, for example: {newParagraph}" +
                $"\"QuestPDF.Settings.License = LicenseType.Community;\" {newParagraph}" +
                $"Learn more on: https://www.questpdf.com/license-configuration.html {newParagraph}";
            
            throw new Exception(exceptionMessage)
            {
                HelpLink = "https://www.questpdf.com/pricing.html"
            };
        }

        internal static ICollection<PreviewerPicture> GeneratePreviewerPictures(IDocument document)
        {
            var canvas = new SkiaPictureCanvas();
            RenderDocument(canvas, document, DocumentSettings.Default);
            return canvas.Pictures;
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
            const int documentId = 0;
            
            var debuggingState = new DebuggingState();
            var useOriginalImages = canvas is ImageCanvas;

            var content = ConfigureContent(document, settings, debuggingState, documentId, useOriginalImages);

            var pageContext = new PageContext();
            RenderPass(pageContext, new FreeCanvas(), content, debuggingState);
            pageContext.ResetPageNumber();
            RenderPass(pageContext, canvas, content, debuggingState);
        }
        
        private static void RenderMergedDocument<TCanvas>(TCanvas canvas, MergedDocument document, DocumentSettings settings)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            var debuggingState = new DebuggingState();
            var useOriginalImages = canvas is ImageCanvas;
            
            var documentParts = Enumerable
                .Range(0, document.Documents.Count)
                .Select(index => new
                {
                    DocumentId = index,
                    Content = ConfigureContent(document.Documents[index], settings, debuggingState, index, useOriginalImages)
                })
                .ToList();

            if (document.PageNumberStrategy == MergedDocumentPageNumberStrategy.Continuous)
            {
                var documentPageContext = new PageContext();

                foreach (var documentPart in documentParts)
                {
                    documentPageContext.SetDocumentId(documentPart.DocumentId);
                    RenderPass(documentPageContext, new FreeCanvas(), documentPart.Content, debuggingState);
                }
                
                documentPageContext.ResetPageNumber();

                foreach (var documentPart in documentParts)
                {
                    documentPageContext.SetDocumentId(documentPart.DocumentId);
                    RenderPass(documentPageContext, canvas, documentPart.Content, debuggingState);   
                }
            }
            else
            {
                foreach (var documentPart in documentParts)
                {
                    var pageContext = new PageContext();
                    pageContext.SetDocumentId(documentPart.DocumentId);
                    
                    RenderPass(pageContext, new FreeCanvas(), documentPart.Content, debuggingState);
                    pageContext.ResetPageNumber();
                    RenderPass(pageContext, canvas, documentPart.Content, debuggingState);
                }
            }
        }

        private static Container ConfigureContent(IDocument document, DocumentSettings settings, DebuggingState debuggingState, int documentIndex, bool useOriginalImages)
        {
            var container = new DocumentContainer();
            document.Compose(container);
            
            var content = container.Compose();
            
            content.ApplyInheritedAndGlobalTexStyle(TextStyle.Default);
            content.ApplyContentDirection(settings.ContentDirection);
            content.ApplyDefaultImageConfiguration(settings.ImageRasterDpi, settings.ImageCompressionQuality, useOriginalImages);
                    
            if (Settings.EnableCaching)
                content.ApplyCaching();

            if (Settings.EnableDebugging)
                content.ApplyDebugging(debuggingState);

            return content;
        }

        private static void RenderPass<TCanvas>(PageContext pageContext, TCanvas canvas, ContainerElement content, DebuggingState? debuggingState)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            content.InjectDependencies(pageContext, canvas);
            content.VisitChildren(x => (x as IStateResettable)?.ResetState());

            while(true)
            {
                var spacePlan = content.Measure(Size.Max);

                if (spacePlan.Type == SpacePlanType.Wrap)
                {
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
                    pageContext.IncrementPageNumber();
                }
                catch (Exception exception)
                {
                    canvas.EndDocument();
                    throw new DocumentDrawingException("An exception occured during document drawing.", exception);
                }

                canvas.EndPage();

                if (pageContext.CurrentPage >= Settings.DocumentLayoutExceptionThreshold)
                {
                    canvas.EndDocument();
                    ThrowLayoutException();
                }
                
                if (spacePlan.Type == SpacePlanType.FullRender)
                    break;
            }

            if (Settings.EnableDebugging)
            {
                ConfigureLayoutOverflowMarker();
            }

            void ApplyLayoutDebugging()
            {
                content.RemoveExistingProxies();

                content.ApplyLayoutOverflowDetection();
                content.Measure(Size.Max);

                var overflowState = content.ExtractElementsOfType<OverflowDebuggingProxy>().FirstOrDefault();
                overflowState.ApplyLayoutOverflowVisualization();
                
                content.ApplyContentDirection();
                content.InjectDependencies(pageContext, canvas);

                content.RemoveExistingProxies();
            }

            void ConfigureLayoutOverflowMarker()
            {
                var layoutOverflowPageMarker = new LayoutOverflowPageMarker();
                    
                content.CreateProxy(child =>
                {
                    layoutOverflowPageMarker.Child = child;
                    return layoutOverflowPageMarker;
                });

                var pageNumbersWithLayoutIssues = content
                    .ExtractElementsOfType<LayoutOverflowVisualization>()
                    .SelectMany(x => x.Flatten())
                    .SelectMany(x => x.Value.VisibleOnPageNumbers)
                    .Distinct();

                layoutOverflowPageMarker.PageNumbersWithLayoutIssues = new HashSet<int>(pageNumbersWithLayoutIssues);
            }
            
            void ThrowLayoutException()
            {
                var message = 
                    $"Composed layout generates infinite document. This may happen in two cases. " +
                    $"1) Your document and its layout configuration is correct but the content takes more than {Settings.DocumentLayoutExceptionThreshold} pages. " +
                    $"In this case, please increase the value {nameof(QuestPDF)}.{nameof(Settings)}.{nameof(Settings.DocumentLayoutExceptionThreshold)} static property. " +
                    $"2) The layout configuration of your document is invalid. Some of the elements require more space than is provided." +
                    $"Please analyze your documents structure to detect this element and fix its size constraints.";
                
                var elementTrace = debuggingState?.BuildTrace() ?? "Debug trace is available only in the DEBUG mode.";
                
                throw new DocumentLayoutException(message, elementTrace);
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

        private static void ApplyCaching(this Container content)
        {
            content.VisitChildren(x =>
            {
                if (x is ICacheable)
                    x.CreateProxy(y => new CacheProxy(y));
            });
        }

        private static void ApplyDebugging(this Container content, DebuggingState? debuggingState)
        {
            if (debuggingState == null)
                return;
            
            content.VisitChildren(x =>
            {
                x.CreateProxy(y => new DebuggingProxy(debuggingState, y));
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
