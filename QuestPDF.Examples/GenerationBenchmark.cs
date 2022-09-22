using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class ProcessRunningTime
    {
        public TimeSpan FluentTime { get; set; }
        public TimeSpan GenerationTime { get; set; }
        public float Size { get; set; }
    }
    
    public class GenerationBenchmark
    {
        public const int TestSize = 4096;
        
        [Test]
        public void BenchmarkAsync()
        {
            RunTest(() => Enumerable
                .Range(0, TestSize)
                .AsParallel() // difference
                .Select(GenerateAndCollect)
                .ToList());
        }
        
        [Test]
        public void BenchmarkSync()
        {
            RunTest(() => Enumerable
                .Range(0, TestSize)
                .Select(GenerateAndCollect)
                .ToList());
        }

        public void RunTest(Func<IEnumerable<ProcessRunningTime>> handler)
        {
            var totalFluentTime = TimeSpan.Zero;
            var totalGenerationTime = TimeSpan.Zero;

            var stopWatch = new Stopwatch();
            
            stopWatch.Start();
            var results = handler();
            stopWatch.Stop();
            
            foreach (var result in results)
            {
                totalFluentTime += result.FluentTime;
                totalGenerationTime += result.GenerationTime;
            }

            Console.WriteLine($"Fluent: {totalFluentTime:g}");
            Console.WriteLine($"Generation: {totalGenerationTime:g}");
            Console.WriteLine($"Total: {stopWatch.Elapsed:g}");
        }
        
        static ProcessRunningTime GenerateAndCollect(int attemptNumber)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            var container = new Container();
            
            container
                .Padding(10)
                .MinimalBox()
                .Border(1)
                .Column(column =>
                {
                    column.Item().Text($"Attempts {attemptNumber}");
                    
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

            var fluentTime = stopwatch.Elapsed;
            
            stopwatch.Reset();
            stopwatch.Start();

            var size = Document
                .Create(x => x.Page(page => page.Content().Element(container)))
                .GeneratePdf()
                .Length;

            var generationTime = stopwatch.Elapsed;
            
            return new ProcessRunningTime
            {
                FluentTime = fluentTime,
                GenerationTime = generationTime,
                Size = size
            };
        }
    }
}