using System;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    static class DocumentGenerator
    {
        const int DocumentLayoutExceptionThreshold = 250;
        private static readonly Watermark Watermark = new Watermark();

        internal static void Generate(Stream stream, IDocument document)
        {
            var content = ElementExtensions.Create(document.Compose);
            var metadata = document.GetMetadata();
            
            using var pdf = SKDocument.CreatePdf(stream, MapMetadata(metadata));
            var totalPages = 1;
            
            while(true)
            {
                var spacePlan = content.Measure(metadata.Size);

                using var skiaCanvas = pdf.BeginPage(metadata.Size.Width, metadata.Size.Height);
                var canvas = new Canvas(skiaCanvas);

                try
                {
                    content.Draw(canvas, metadata.Size);
                }
                catch (Exception exception)
                {
                    pdf.Close();
                    stream.Close();
                    
                    throw new DocumentDrawingException("An exception occured during document drawing.", exception);
                }

                pdf.EndPage();

                if (totalPages >= DocumentLayoutExceptionThreshold)
                {
                    pdf.Close();
                    stream.Close();

                    throw new DocumentLayoutException("Composed layout generates infinite document.");
                }
                
                if (spacePlan is FullRender)
                    break;

                totalPages++;
            }
            
            pdf.Close();
            stream.Dispose();
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