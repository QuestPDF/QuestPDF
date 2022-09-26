using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class TableBenchmark
    {
        [Test]
        public void Benchmark()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(PageSizes.A4)
                .ShowResults()
                .MaxPages(10_000)
                //.EnableCaching(true)
                .EnableDebugging(false)
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .MinimalBox()
                        .Border(1)
                        .Table(table =>
                        {
                            const int numberOfRows = 100_000;
                            const int numberOfColumns = 10;
                            
                            table.ColumnsDefinition(columns =>
                            {
                                foreach (var _ in Enumerable.Range(0, numberOfColumns))
                                    columns.RelativeColumn();
                            });

                            foreach (var row in Enumerable.Range(0, numberOfRows))
                            foreach (var column in Enumerable.Range(0, numberOfColumns))
                                table.Cell().Background(Placeholders.BackgroundColor()).Padding(5).Text($"{row}_{column}");
                        });
                });
        }
    }
}