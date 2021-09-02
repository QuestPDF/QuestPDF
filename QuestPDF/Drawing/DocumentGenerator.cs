using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Drawing.Exceptions;
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

            ApplyElementProxy<CacheProxy>(content);
            
            RenderPass(pageContext, new FreeCanvas(), content, metadata);
            RenderPass(pageContext, canvas, content, metadata);
        }
        
        private static void RenderPass<TCanvas>(PageContext pageContext, TCanvas canvas, Container content, DocumentMetadata documentMetadata)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            content.HandleVisitor(x => x?.Initialize(pageContext, canvas));
            content.HandleVisitor(x => (x as IStateResettable)?.ResetState());
            
            canvas.BeginDocument();

            var currentPage = 1;
            
            while(true)
            {
                pageContext.SetPageNumber(currentPage);
                var spacePlan = content.Measure(Size.Max);

                if (spacePlan.Type == SpacePlanType.Wrap)
                    break;

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
                    throw new DocumentLayoutException("Composed layout generates infinite document.");
                }
                
                if (spacePlan.Type == SpacePlanType.FullRender)
                    break;

                currentPage++;
            }
            
            canvas.EndDocument();
        }

        private static void ApplyElementProxy<TProxy>(Container content) where TProxy : ElementProxy, new()
        {
            content.HandleVisitor(x => x.CreateProxy(y => new TProxy()
            {
                Child = y
            }));
        }
    }
}