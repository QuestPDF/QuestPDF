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
        [Ignore("This test takes a lot of time to run")]
        public void BenchmarkAsync()
        {
            RunTest(() => Enumerable
                .Range(0, TestSize)
                .AsParallel() // difference
                .Select(GenerateAndCollect)
                .ToList());
        }
        
        [Test]
        [Ignore("This test takes a lot of time to run")]
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
            var fluentTime = TimeSpan.Zero;

            var document = Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Content()
                        .Padding(10)
                        .Shrink()
                        .Border(1)
                        .Column(column =>
                        {
                            var fluentTimeStopwatch = new Stopwatch();
                            fluentTimeStopwatch.Start();
                            
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
                            
                            fluentTime = fluentTimeStopwatch.Elapsed;
                        });
                });
            });

            var generationTimeStopWatch = new Stopwatch();
            
            generationTimeStopWatch.Start();
            var size = document.GeneratePdf().Length;
            var generationTime = generationTimeStopWatch.Elapsed;
            
            return new ProcessRunningTime
            {
                FluentTime = fluentTime,
                GenerationTime = generationTime,
                Size = size
            };
        }
    }
}