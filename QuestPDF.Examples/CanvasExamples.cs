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
                .PageSize(175, 100)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background(Colors.Grey.Lighten2)
                        .Padding(25)
                        .Box()
                        .Layers(layers =>
                        {
                            layers.Layer().Canvas((canvas, size) =>
                            {
                                DrawRoundedRectangle(Colors.White, false);
                                DrawRoundedRectangle(Colors.Blue.Darken2, true);

                                void DrawRoundedRectangle(string color, bool isStroke)
                                {
                                    using var paint = new SKPaint
                                    {
                                        Color = SKColor.Parse(color),
                                        IsStroke = isStroke,
                                        StrokeWidth = 2,
                                        IsAntialias = true
                                    };
                                
                                    canvas.DrawRoundRect(0, 0, size.Width, size.Height, 20, 20, paint);
                                }
                            });
                            
                            layers
                                .PrimaryLayer()
                                .PaddingVertical(10)
                                .PaddingHorizontal(20)
                                .Text("Sample text", TextStyle.Default.Size(16).Color(Colors.Blue.Darken2).SemiBold());
                        });
                });
        }
    }
}