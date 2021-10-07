using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    static class DocumentGenerator
    {
        internal static void GeneratePdf(Stream stream, IDocument document)
        {
            var metadata = document.GetMetadata();
            var canvas = new PdfCanvas(stream, metadata);
            RenderDocument(canvas, document);
        }
        
        internal static ICollection<byte[]> GenerateImages(IDocument document)
        {
            var metadata = document.GetMetadata();
            var canvas = new ImageCanvas(metadata);
            RenderDocument(canvas, document);

            return canvas.Images;
        }

        private static void RenderDocument<TCanvas>(TCanvas canvas, IDocument document)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            var container = new DocumentContainer();
            document.Compose(container);
            var content = container.Compose();
            
            var metadata = document.GetMetadata();
            var pageContext = new PageContext();

            DebuggingState debuggingState = null;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                debuggingState = ApplyDebugging(content);
            }
            else
            {
                ApplyCaching(content);
            }

            RenderPass(pageContext, new FreeCanvas(), content, metadata, debuggingState);
            RenderPass(pageContext, canvas, content, metadata, debuggingState);
        }
        
        internal static void RenderPass<TCanvas>(PageContext pageContext, TCanvas canvas, Container content, DocumentMetadata documentMetadata, DebuggingState? debuggingState)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            content.HandleVisitor(x => x?.Initialize(pageContext, canvas));
            content.HandleVisitor(x => (x as IStateResettable)?.ResetState());
            
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

                if (currentPage >= documentMetadata.DocumentLayoutExceptionThreshold)
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
                throw new DocumentLayoutException("Composed layout generates infinite document.")
                {
                    ElementTrace = debuggingState?.BuildTrace() ?? "Debug trace is available only in the DEBUG mode."
                };
            }
        }

        private static void ApplyCaching(Container content)
        {
            content.HandleVisitor(x =>
            {
                if (x is ICacheable)
                    x.CreateProxy(y => new CacheProxy(y));
            });
        }

        private static DebuggingState ApplyDebugging(Container content)
        {
            var debuggingState = new DebuggingState();

            content.HandleVisitor(x =>
            {
                x.CreateProxy(y => new DebuggingProxy(debuggingState, x));
            });

            return debuggingState;
        }
    }
}