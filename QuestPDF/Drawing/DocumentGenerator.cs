using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    static class DocumentGenerator
    {
        internal static void GeneratePdf(Stream stream, IDocument document)
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
                    throw new DocumentDrawingException("An exception occured during document drawing.", exception);
                }

                pdf.EndPage();

                if (totalPages >= metadata.DocumentLayoutExceptionThreshold)
                {
                    pdf.Close();
                    throw new DocumentLayoutException("Composed layout generates infinite document.");
                }
                
                if (spacePlan is FullRender)
                    break;

                totalPages++;
            }
            
            pdf.Close();
        }

        internal static IEnumerable<byte[]> GenerateImages(IDocument document)
        {
            var content = ElementExtensions.Create(document.Compose);
            var metadata = document.GetMetadata();

            var totalPages = 1;

            while (true)
            {
                var spacePlan = content.Measure(metadata.Size);
                byte[] result;

                try
                {
                    result = RenderPage(content);
                }
                catch (Exception exception)
                {
                    throw new DocumentDrawingException("An exception occured during document drawing.", exception);
                }

                yield return result;

                if (totalPages >= metadata.DocumentLayoutExceptionThreshold)
                {
                    throw new DocumentLayoutException("Composed layout generates infinite document.");
                }

                if (spacePlan is FullRender)
                    break;

                totalPages++;
            }

            byte[] RenderPage(Element element)
            {
                // scale the result so it is more readable
                var scalingFactor = metadata.RasterDpi / (float) PageSizes.PointsPerInch;
                
                var imageInfo = new SKImageInfo((int) (metadata.Size.Width * scalingFactor), (int) (metadata.Size.Height * scalingFactor));
                using var surface = SKSurface.Create(imageInfo);
                surface.Canvas.Scale(scalingFactor);

                var canvas = new Canvas(surface.Canvas);
                element?.Draw(canvas, metadata.Size);

                surface.Canvas.Save();
                return surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).ToArray();
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