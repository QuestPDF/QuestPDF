using System.IO;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class PdfCanvas : SkiaCanvasBase
    {
        private SKDocument Document { get; }

        public PdfCanvas(Stream stream, DocumentMetadata documentMetadata)
        {
            Document = SKDocument.CreatePdf(stream, MapMetadata(documentMetadata));
        }

        ~PdfCanvas()
        {
            Document.Dispose();
        }
        
        public override void BeginDocument()
        {
            
        }

        public override void EndDocument()
        {
            Canvas.Dispose();
            
            Document.Close();
            Document.Dispose();
        }

        public override void BeginPage(Size size)
        {
            Canvas = Document.BeginPage(size.Width, size.Height);
        }

        public override void EndPage()
        {
            Document.EndPage();
            Canvas.Dispose();
        }
        
        private static SKDocumentPdfMetadata MapMetadata(DocumentMetadata metadata)
        {
            return new SKDocumentPdfMetadata
            {
                Title = metadata.Title,
                Author = metadata.Author,
                Subject = metadata.Subject,
                Keywords = metadata.Keywords,
                Creator = metadata.Creator,
                Producer = metadata.Producer,
                
                Creation = metadata.CreationDate,
                Modified = metadata.ModifiedDate,
                
                RasterDpi = metadata.RasterDpi,
                EncodingQuality = metadata.ImageQuality,
                PdfA = metadata.PdfA
            };
        }
    }
}