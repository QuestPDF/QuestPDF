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
            // do not extract to another method, as it will cause the SkText objects
            // to be disposed before the SkPdfDocument is created
            using var title = new SkText(documentMetadata.Title);
            using var author = new SkText(documentMetadata.Author);
            using var subject = new SkText(documentMetadata.Subject);
            using var keywords = new SkText(documentMetadata.Keywords);
            using var creator = new SkText(documentMetadata.Creator);
            using var producer = new SkText(documentMetadata.Producer);
            
            var internalMetadata = new SkPdfDocumentMetadata
            {
                Title = title,
                Author = author,
                Subject = subject,
                Keywords = keywords,
                Creator = creator,
                Producer = producer,
                
                CreationDate = new SkDateTime(documentMetadata.CreationDate),
                ModificationDate = new SkDateTime(documentMetadata.ModifiedDate),
                
                RasterDPI = documentSettings.ImageRasterDpi,
                ImageEncodingQuality = documentSettings.ImageCompressionQuality.ToQualityValue(),
                
                SupportPDFA = documentSettings.PdfA,
                CompressDocument = documentSettings.CompressDocument
            };
            
            try
            {
                return SkPdfDocument.Create(stream, internalMetadata);
            }
            catch (TypeInitializationException exception)
            {
                throw new InitializationException("PDF", exception);
            }
        }
    }
}