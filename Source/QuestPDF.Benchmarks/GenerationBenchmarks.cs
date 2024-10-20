using BenchmarkDotNet.Attributes;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;

namespace QuestPDF.Benchmarks;

[Config(typeof(Config))]
public class GenerationBenchmarks
{
    private const int TestSize = 128;
    
    public GenerationBenchmarks()
    {
        Settings.License = LicenseType.Community;
    }
    
    [Benchmark]
    public void Fluent_Async()
    {
        Parallel.For(0, TestSize, i => CreateColumn(new ColumnDescriptor(), i));
    }
    
    [Benchmark]
    public void Fluent_Sync()
    {
        for (var i = 0; i < TestSize; i++)
        {
            CreateColumn(new ColumnDescriptor(), i);
        }
    }

    private IDocument? _document;
    [GlobalSetup(Targets = new[] { nameof(Generation_Async), nameof(Generation_Sync) })]
    public void GenerationSetup()
    {
        _document = CreateDocument();
    }

    [Benchmark]
    public void Generation_Async()
    {
        Parallel.For(0, TestSize, _ => _document!.GeneratePdf());
    }
    
    [Benchmark]
    public void Generation_Sync()
    {
        for (var i = 0; i < TestSize; i++)
        {
            _document!.GeneratePdf();
        }
    }
    
    private static void CreateColumn(ColumnDescriptor column, int i)
    {
        column.Item().Text($"Attempts {i}");

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
    }
    
    private static IDocument CreateDocument()
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Content()
                    .Padding(10)
                    .Shrink()
                    .Border(1)
                    .Column(column => CreateColumn(column, 0));
            });
        });
    }
}