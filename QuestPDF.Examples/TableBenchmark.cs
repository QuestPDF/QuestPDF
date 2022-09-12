using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class TableBenchmark
    {
        [Test]
        public void Benchmark()
        {
            GenerateAndCollect();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var _ in Enumerable.Range(0, 10000))
            {
                GenerateAndCollect();
            }
            
            stopwatch.Stop();
            
            Console.WriteLine($"Execution time: {stopwatch.Elapsed:g}");

            void GenerateAndCollect()
            {
                var container = new Container();
                
                container
                    .Padding(10)
                    .MinimalBox()
                    .Border(1)
                    .Column(column =>
                    {
                        const int numberOfRows = 100;
                        const int numberOfColumns = 10;

                        for (var y = 0; y < numberOfRows; y++)
                        {
                            column.Item().Row(row =>
                            {
                                for (var x = 0; x < numberOfColumns; x++)
                                {
                                    row.RelativeItem()
                                        
                                        .Background(Colors.Red.Lighten5)
                                        .Padding(3)
                                        
                                        .Background(Colors.Red.Lighten4)
                                        .Padding(3)
                                        
                                        .Background(Colors.Red.Lighten3)
                                        .Padding(3)
                                        
                                        .Background(Colors.Red.Lighten2)
                                        .Padding(3)
                                        
                                        .Background(Colors.Red.Lighten1)
                                        .Padding(3)
                                        
                                        .Background(Colors.Red.Medium)
                                        .Padding(3)
                                        
                                        .Background(Colors.Red.Darken1)
                                        .Padding(3)
                                        
                                        .Background(Colors.Red.Darken2)
                                        .Padding(3)
                                        
                                        .Background(Colors.Red.Darken3)
                                        .Padding(3)
                                        
                                        .Background(Colors.Red.Darken4)
                                        .Height(3);
                                }
                            });
                        }  
                    });

                ElementCacheManager.Collect(container);
            }
        }
    }
}