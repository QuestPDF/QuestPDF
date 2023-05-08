using System;
using System.Collections.Generic;
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
        
        internal static ICollection<byte[]> GenerateImages(IDocument document)
        {
            ValidateLicense();
            
            var settings = document.GetSettings();
            var canvas = new ImageCanvas(settings);
            RenderDocument(canvas, document, settings);

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
                $"We love and highly appreciate the .NET Community, and therefore the vast majority of users are welcome to use the library completely for free under the QuestPDF Community MIT license. {newParagraph}" +
                $"However, if you are consuming the QuestPDF library as a Direct Package Dependency for usage in a Closed Source software in the capacity of a for-profit company/individual with more than 1M USD annual gross revenue, you must purchase the QuestPDF Professional or Enterprise License, depending on the number of software developers. {newParagraph}" +
                $"If you still want to support library development, please consider purchasing the Professional License. {newParagraph}" +
                $"For evaluation purposes, feel free to use the QuestPDF Community License in a non-production environment. {newParagraph}" +
                $"Please refer to the QuestPDF License and Pricing webpage for more details. (https://www.questpdf.com/pricing.html) {newParagraph}" +
                $"If you are an existing QuestPDF user and for any reason cannot update, you can stay with the 2022.12.X release with the extended quality support but without any new features, improvements, or optimizations. That release will always be available under the MIT license, free for commercial usage. {newParagraph}" +
                $"The library does not require any license key. " +
                $"We trust our users, and therefore the process is simple. " +
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
        
        internal static void RenderDocument<TCanvas>(TCanvas canvas, IDocument document, DocumentSettings settings)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            var container = new DocumentContainer();
            document.Compose(container);
            var content = container.Compose();
            
            ApplyInheritedAndGlobalTexStyle(content, TextStyle.Default);
            ApplyContentDirection(content, settings.ContentDirection);
            ApplyDefaultImageConfiguration(content, settings);
            
            var debuggingState = Settings.EnableDebugging ? ApplyDebugging(content) : null;
            
            if (Settings.EnableCaching)
                ApplyCaching(content);

            var pageContext = new PageContext();
            RenderPass(pageContext, new FreeCanvas(), content, debuggingState);
            RenderPass(pageContext, canvas, content, debuggingState);
        }
        
        internal static void RenderPass<TCanvas>(PageContext pageContext, TCanvas canvas, Container content, DebuggingState? debuggingState)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            InjectDependencies(content, pageContext, canvas);
            content.VisitChildren(x => (x as IStateResettable)?.ResetState());
            
            canvas.BeginDocument();

            var currentPage = 1;
            
            while(true)
            {
                pageContext.SetPageNumber(currentPage);
                debuggingState?.Reset();
                
                var spacePlan = content.Measure(Size.Max);

                if (spacePlan.Type == SpacePlanType.Wrap)
                {
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

                if (currentPage >= Settings.DocumentLayoutExceptionThreshold)
                {
                    canvas.EndDocument();
                    ThrowLayoutException();
                }
                
                if (spacePlan.Type == SpacePlanType.FullRender)
                    break;

                currentPage++;
            }
            
            canvas.EndDocument();

            void ThrowLayoutException()
            {
                var message = $"Composed layout generates infinite document. This may happen in two cases. " +
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

        private static void ApplyCaching(Container content)
        {
            content.VisitChildren(x =>
            {
                if (x is ICacheable)
                    x.CreateProxy(y => new CacheProxy(y));
            });
        }

        private static DebuggingState ApplyDebugging(Container content)
        {
            var debuggingState = new DebuggingState();

            content.VisitChildren(x =>
            {
                x.CreateProxy(y => new DebuggingProxy(debuggingState, y));
            });

            return debuggingState;
        }
        
        internal static void ApplyContentDirection(this Element? content, ContentDirection direction)
        {
            if (content == null)
                return;

            if (content is ContentDirectionSetter contentDirectionSetter)
            {
                ApplyContentDirection(contentDirectionSetter.Child, contentDirectionSetter.ContentDirection);
                return;
            }

            if (content is IContentDirectionAware contentDirectionAware)
                contentDirectionAware.ContentDirection = direction;
            
            foreach (var child in content.GetChildren())
                ApplyContentDirection(child, direction);
        }
        
        internal static void ApplyDefaultImageConfiguration(this Element? content, DocumentSettings settings)
        {
            content.VisitChildren(x =>
            {
                if (x is QuestPDF.Elements.Image image)
                {
                    image.TargetDpi ??= settings.RasterDpi;
                    image.CompressionQuality ??= settings.ImageCompressionQuality;
                }

                if (x is QuestPDF.Elements.DynamicImage dynamicImage)
                {
                    dynamicImage.TargetDpi ??= settings.RasterDpi;
                    dynamicImage.CompressionQuality ??= settings.ImageCompressionQuality;
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
