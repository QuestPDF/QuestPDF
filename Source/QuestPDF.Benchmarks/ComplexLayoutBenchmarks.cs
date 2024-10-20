using BenchmarkDotNet.Attributes;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Benchmarks;

[Config(typeof(Config))]
public class ComplexLayoutBenchmarks
{
    public ComplexLayoutBenchmarks()
    {
        Settings.License = LicenseType.Community;
    }
    
    [Benchmark]
    public void Bench()
    {
        RenderingTest
            .Create()
            .PageSize(PageSizes.A4)
            .ProducePdf()
            .Render(x => GenerateStructure(x, 12));
    }
    
    private void GenerateStructure(IContainer container, int level)
    {
        if (level <= 0)
        {
            container.Background(Placeholders.BackgroundColor()).Height(10);
            return;
        }

        level--;

        if (level % 3 == 0)
        {
            container
                .Border(level / 4f)
                .BorderColor(Colors.Black)
                .Row(row =>
                {
                    row.RelativeItem().Element(x => GenerateStructure(x, level));
                    row.RelativeItem().Element(x => GenerateStructure(x, level));
                });
        }
        else
        {
            container
                .Border(level / 4f)
                .BorderColor(Colors.Black)
                .Column(column =>
                {
                    column.Item().Element(x => GenerateStructure(x, level));
                    column.Item().Element(x => GenerateStructure(x, level));
                });
        }
    }
}