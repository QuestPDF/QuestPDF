using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Elements.Text;
using QuestPDF.Elements.Text.Items;
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

            ApplyDefaultTextStyle(content, container.DefaultTextStyle);
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
                var spacePlan = content.Measure(Size.Max) as Size;

                if (spacePlan == null)
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
                    throw new DocumentLayoutException("Composed layout generates infinite document.");
                }
                
                if (spacePlan is FullRender)
                    break;

                currentPage++;
            }
            
            canvas.EndDocument();
            
            void ThrowLayoutException()
            {
                throw new DocumentLayoutException("Composed layout generates infinite document.");
            }
        }

        private static void ApplyDefaultTextStyle(Element content, TextStyle documentDefaultTextStyle)
        {
            documentDefaultTextStyle.ApplyGlobalStyle(TextStyle.LibraryDefault);
            
            content.HandleVisitor(element =>
            {
                var text = element as TextBlock;
                
                if (text == null)
                    return;

                foreach (var child in text.Children)
                {
                    if (child is TextBlockSpan textSpan)
                        textSpan.Style.ApplyGlobalStyle(documentDefaultTextStyle);

                    if (child is TextBlockElement textElement)
                        ApplyDefaultTextStyle(textElement.Element, documentDefaultTextStyle);
                }
            });
        }
    }
}