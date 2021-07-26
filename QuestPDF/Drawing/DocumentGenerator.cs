using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var container = new DocumentContainer();
            document.Compose(container);
            var content = container.Compose();
            
            var metadata = document.GetMetadata();
            var pageContext = new PageContext();

            var timer = new Stopwatch();
            
            timer.Start();
            RenderDocument(pageContext, new FreeCanvas(), content, metadata);
            
            Console.WriteLine($"First rendering pass: {timer.Elapsed:g}");
            timer.Restart();
            
            var canvas = new PdfCanvas(stream, metadata);
            RenderDocument(pageContext, canvas, content, metadata);
            
            Console.WriteLine($"Second rendering pass: {timer.Elapsed:g}");
        }

        private static void RenderDocument<TCanvas>(PageContext pageContext, TCanvas canvas, Container content, DocumentMetadata documentMetadata)
            where TCanvas : ICanvas, IRenderingCanvas
        {
            content.HandleVisitor(x => x.Initialize(pageContext, canvas));
            content.HandleVisitor(x => (x as IStateResettable)?.ResetState());
            
            canvas.BeginDocument();

            var currentPage = 1;
            
            while(true)
            {
                pageContext.SetPageNumber(currentPage);
                var spacePlan = content.Measure(Size.Max) as Size;

                if (spacePlan == null)
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
                
                if (spacePlan is FullRender)
                    break;

                currentPage++;
            }
            
            canvas.EndDocument();
        }
        
        internal static IEnumerable<byte[]> GenerateImages(IDocument document)
        {
            return null;
            
            // var container = new DocumentContainer();
            // document.Compose(container);
            // var content = container.Compose();
            //
            // var metadata = document.GetMetadata();
            //
            // var currentPage = 1;
            //
            // while (true)
            // {
            //     var spacePlan = content.Measure(null, Size.Max) as Size;
            //     byte[] result;
            //
            //     if (spacePlan == null)
            //         break;
            //     
            //     try
            //     {
            //         result = RenderPage(spacePlan, content);
            //     }
            //     catch (Exception exception)
            //     {
            //         throw new DocumentDrawingException("An exception occured during document drawing.", exception);
            //     }
            //
            //     yield return result;
            //
            //     if (currentPage >= metadata.DocumentLayoutExceptionThreshold)
            //     {
            //         throw new DocumentLayoutException("Composed layout generates infinite document.");
            //     }
            //
            //     if (spacePlan is FullRender)
            //         break;
            //
            //     currentPage++;
            // }

            // byte[] RenderPage(Size size, Element element)
            // {
            //     // scale the result so it is more readable
            //     var scalingFactor = metadata.RasterDpi / (float) PageSizes.PointsPerInch;
            //     
            //     var imageInfo = new SKImageInfo((int) (size.Width * scalingFactor), (int) (size.Height * scalingFactor));
            //     using var surface = SKSurface.Create(imageInfo);
            //     surface.Canvas.Scale(scalingFactor);
            //
            //     var canvas = new SkiaCanvasBase(surface.Canvas, new Dictionary<string, int>());
            //     element?.Draw(canvas, size);
            //
            //     surface.Canvas.Save();
            //     return surface.Snapshot().Encode(SKEncodedImageFormat.Png, 100).ToArray();
            // }
        }
    }
}