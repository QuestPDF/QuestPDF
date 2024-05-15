using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class GridBenchmark
    {
        [Test]
        [Ignore("This test takes a lot of time to run")]
        public void Benchmark()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(PageSizes.A4)
                .ShowResults()
                .MaxPages(10_000)
                .EnableCaching(true)
                .EnableDebugging(false)
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Grid(grid =>
                        {
                            const int numberOfRows = 100_000;
                            const int numberOfColumns = 10;
                            
                            grid.Columns(numberOfColumns);
                            
                            foreach (var row in Enumerable.Range(0, numberOfRows))
                            foreach (var column in Enumerable.Range(0, numberOfColumns))
                                grid.Item().Background(Placeholders.BackgroundColor()).Padding(5).Text($"{row}_{column}");
                        });
                });
        }
    }
}