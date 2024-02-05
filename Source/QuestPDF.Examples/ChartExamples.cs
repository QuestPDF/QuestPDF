using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Microcharts;
using ScottPlot;
using SkiaSharp;
using Colors = QuestPDF.Helpers.Colors;
using Orientation = Microcharts.Orientation;

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
        
        [Test]
        public void ScottPlotChart()
        {
            RenderingTest
                .Create()
                .PageSize(400, 300)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background(Colors.White)
                        .Padding(25)
                        .Canvas((canvas, availableSpace) =>
                        {
                            var points = Enumerable
                                .Range(0, 100)
                                .Select(x => new Coordinates(x, Math.Sin(x / 10f)))
                                .ToArray();
                            
                            using var plot = new Plot();
                            plot.Add.Scatter(points, Color.FromHex("#009688"));

                            canvas.Save();
                            canvas.ClipRect(new SKRect(0, 0, availableSpace.Width, availableSpace.Height));
                            plot.Render(canvas, (int)availableSpace.Width, (int)availableSpace.Height);
                            canvas.Restore();
                        });
                });
        }
    }
}