using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
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
                .PageSize(400, 600)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background(Colors.White)
                        .Padding(25)
                        .Column(column =>
                        {
                            column
                                .Item()
                                .PaddingBottom(10)
                                .Text("Chart example")
                                .FontSize(20)
                                .SemiBold()
                                .FontColor(Colors.Blue.Medium);
                            
                            column
                                .Item()
                                .Border(1)
                                .ExtendHorizontal()
                                .Height(300)
                                .Canvas((canvas, size) =>
                                {
                                    var chart = new BarChart
                                    {
                                        Entries = entries,
       
                                        LabelOrientation = Orientation.Horizontal,
                                        ValueLabelOrientation = Orientation.Horizontal,
                                        
                                        IsAnimated = false,
                                    };
                                    
                                    chart.DrawContent(canvas, (int)size.Width, (int)size.Height);
                                });
                        });
                });
        }
    }
}