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
            CheckIfStreamIsCompatible(stream);
            
            var metadata = document.GetMetadata();
            var canvas = new PdfCanvas(stream, metadata);
            RenderDocument(canvas, document);
        }
        
        internal static void GenerateXps(Stream stream, IDocument document)
        {
            CheckIfStreamIsCompatible(stream);
            
            var metadata = document.GetMetadata();
            var canvas = new XpsCanvas(stream, metadata);
            RenderDocument(canvas, document);
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
            var metadata = document.GetMetadata();
            var canvas = new ImageCanvas(metadata);
            RenderDocument(canvas, document);

            return canvas.Images;
        }

        internal static ICollection<PreviewerPicture> GeneratePreviewerPictures(IDocument document)
        {
            var canvas = new SkiaPictureCanvas();
            RenderDocument(canvas, document);
            return canvas.Pictures;
        }
        
        internal static void RenderDocument<TCanvas>(TCanvas canvas, IDocument document)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            var container = new DocumentContainer();
            document.Compose(container);
            var content = container.Compose();
            ApplyDefaultTextStyle(content, TextStyle.LibraryDefault);
            
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
            content.VisitChildren(x => x?.Initialize(pageContext, canvas));
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

        internal static void ApplyDefaultTextStyle(this Element? content, TextStyle documentDefaultTextStyle)
        {
            if (content == null)
                return;
            
            if (content is TextBlock textBlock)
            {
                foreach (var textBlockItem in textBlock.Items)
                {
                    if (textBlockItem is TextBlockSpan textSpan)
                    {
                        textSpan.Style.ApplyGlobalStyle(documentDefaultTextStyle);
                    }
                    else if (textBlockItem is TextBlockElement textElement)
                    {
                        ApplyDefaultTextStyle(textElement.Element, documentDefaultTextStyle);
                    }
                }
                
                return;
            }

            if (content is DynamicHost dynamicHost)
                dynamicHost.TextStyle.ApplyGlobalStyle(documentDefaultTextStyle);
            
            var targetTextStyle = documentDefaultTextStyle;
            
            if (content is DefaultTextStyle defaultTextStyleElement)
            {
                defaultTextStyleElement.TextStyle.ApplyParentStyle(documentDefaultTextStyle);
                targetTextStyle = defaultTextStyleElement.TextStyle;
            }
            
            foreach (var child in content.GetChildren())
                ApplyDefaultTextStyle(child, targetTextStyle);
        }
    }
}