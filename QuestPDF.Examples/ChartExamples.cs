using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microcharts;
using SkiaSharp;

namespace QuestPDF.Examples
{    
    public class ChartExample
    {
        [Test]
        public void MicrochartChart()
        {
            var entries = new[]
            {
                new ChartEntry(212)
                {
                    Label = "UWP",
                    ValueLabel = "112",
                    Color = SKColor.Parse("#2c3e50")
                },
                new ChartEntry(248)
                {
                    Label = "Android",
                    ValueLabel = "648",
                    Color = SKColor.Parse("#77d065")
                },
                new ChartEntry(128)
                {
                    Label = "iOS",
                    ValueLabel = "428",
                    Color = SKColor.Parse("#b455b6")
                },
                new ChartEntry(514)
                {
                    Label = "Forms",
                    ValueLabel = "214",
                    Color = SKColor.Parse("#3498db")
                }
            };

            RenderingTest
                .Create()
                .PageSize(300, 300)
                .ShowResults()
                .Render(container =>
                {
                    container.Extend().Canvas((canvas, size) => 
                    {
                        var bar = new BarChart
                        {
                            Entries = entries,
                            IsAnimated = false,
                        };
                        bar.Draw(canvas, (int)size.Width, (int)size.Height);
                    });
                });
        }
    }
}