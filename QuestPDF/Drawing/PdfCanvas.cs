using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class PdfCanvas : SkiaDocumentCanvasBase
    {
        public PdfCanvas(Stream stream, DocumentMetadata documentMetadata) 
            : base(CreatePdf(stream, documentMetadata))
        {
            
        }

        private static SKDocument CreatePdf(Stream stream, DocumentMetadata documentMetadata)
        {
            try
            {
                return SKDocument.CreatePdf(stream, MapMetadata(documentMetadata));
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("PDF", exception);
            }
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