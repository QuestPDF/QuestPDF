using Microcharts;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Examples
{
    public class CanvasExamples
    {
        [Test]
        public void BorderRadius()
        {
            RenderingTest
                .Create()
                .PageSize(400, 600)
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background(Colors.White)
                        .Padding(25)
                        .Box()
                        .Layers(layers =>
                        {
                            layers.PrimaryLayer().Padding(10).Text("Sample text");
                            
                            layers.Layer().Canvas((canvas, size) =>
                            {
                                using var paint = new SKPaint
                                {
                                    Color = SKColor.Parse(Colors.Black),
                                    IsStroke = true,
                                    StrokeWidth = 1
                                };
                                
                                canvas.DrawRoundRect(0, 0, size.Width, size.Height, 20, 20, paint);
                            });
                        });
                });
        }
    }
}