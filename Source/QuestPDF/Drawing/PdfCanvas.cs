using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class PdfCanvas : SkiaDocumentCanvasBase
    {
        public PdfCanvas(Stream stream, DocumentMetadata documentMetadata, DocumentSettings documentSettings) 
            : base(CreatePdf(stream, documentMetadata, documentSettings))
        {
            
        }

        private static SKDocument CreatePdf(Stream stream, DocumentMetadata documentMetadata, DocumentSettings documentSettings)
        {
            try
            {
                return SKDocument.CreatePdf(stream, MapMetadata(documentMetadata, documentSettings));
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("PDF", exception);
            }
        }

        private static SKDocumentPdfMetadata MapMetadata(DocumentMetadata metadata, DocumentSettings documentSettings)
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
                
                RasterDpi = documentSettings.ImageRasterDpi,
                EncodingQuality = documentSettings.ImageCompressionQuality.ToQualityValue(),
                PdfA = documentSettings.PdfA
            };
        }
    }
}