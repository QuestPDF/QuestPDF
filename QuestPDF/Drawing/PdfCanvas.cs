using System.IO;
using QuestPDF.Helpers;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class PdfCanvas : SkiaDocumentCanvasBase
    {
        public PdfCanvas(Stream stream, DocumentMetadata documentMetadata) 
            : base(SKDocument.CreatePdf(stream, MapMetadata(documentMetadata)))
        {
            
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