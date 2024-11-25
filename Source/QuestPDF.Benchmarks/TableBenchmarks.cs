using BenchmarkDotNet.Attributes;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Benchmarks;

[Config(typeof(Config))]
public class TableBenchmarks
{
    public TableBenchmarks()
    {
        Settings.License = LicenseType.Community;
    }
    
    [Benchmark]
    public void Benchmark()
    {
        RenderingTest
            .Create()
            .ProducePdf()
            .PageSize(PageSizes.A4)
            .MaxPages(100)
            .EnableCaching()
            .EnableDebugging(false)
            .Render(container =>
            {
                container
                    .Padding(10)
                    .Shrink()
                    .Border(1)
                    .Table(table =>
                    {
                        const int numberOfRows = 1_500;
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