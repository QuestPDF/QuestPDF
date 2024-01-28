using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Drawing
{
    internal sealed class PdfCanvas : SkiaDocumentCanvasBase
    {
        public PdfCanvas(SkWriteStream stream, DocumentMetadata documentMetadata, DocumentSettings documentSettings) 
            : base(CreatePdf(stream, documentMetadata, documentSettings))
        {
            
        }

        private static SkDocument CreatePdf(SkWriteStream stream, DocumentMetadata documentMetadata, DocumentSettings documentSettings)
        {
            try
            {
                return SkPdfDocument.Create(stream, MapMetadata(documentMetadata, documentSettings));
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("PDF", exception);
            }
        }

        private static SkPdfDocumentMetadata MapMetadata(DocumentMetadata metadata, DocumentSettings documentSettings)
        {
            return new SkPdfDocumentMetadata
            {
                Title = metadata.Title,
                Author = metadata.Author,
                Subject = metadata.Subject,
                Keywords = metadata.Keywords,
                Creator = metadata.Creator,
                Producer = metadata.Producer,
                
                CreationDate = new SkDateTime(metadata.CreationDate),
                ModificationDate = new SkDateTime(metadata.ModifiedDate),
                
                RasterDPI = documentSettings.ImageRasterDpi,
                ImageEncodingQuality = documentSettings.ImageCompressionQuality.ToQualityValue(),
                
                SupportPDFA = documentSettings.PdfA,
                CompressDocument = documentSettings.CompressDocument
            };
        }
    }
}