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
                        .MinimalBox()
                        .Layers(layers =>
                        {
                            layers.Layer().SkiaSharpCanvas((canvas, size) =>
                            {
                                DrawRoundedRectangle(Colors.White, false);
                                DrawRoundedRectangle(Colors.Blue.Darken2, true);

                                void DrawRoundedRectangle(Color color, bool isStroke)
                                {
                                    using var paint = new SKPaint
                                    {
                                        Color = new SKColor(color.Hex),
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
                                .Text("Sample text")
                                .FontSize(16)
                                .FontColor(Colors.Blue.Darken2)
                                .SemiBold();
                        });
                });
        }
    }
}