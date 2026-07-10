using BenchmarkDotNet.Attributes;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Benchmarks;

/// <summary>
/// Measures the cost of rendering text-heavy documents in two extreme shapes, both sourced from
/// the same book (Resources/book.txt.zip) so the two shapes are directly comparable:
/// a single Text element containing the entire book (one huge layout calculation),
/// and one Text element per line of the book (tens of thousands of independent layout calculations).
/// </summary>
public class TextBenchmarks
{
    private string BookText { get; set; }
    private IReadOnlyList<string> BookLines { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        BenchmarkEnvironment.ConfigureQuestPdf();

        var sourcePath = Path.Combine(AppContext.BaseDirectory, "Resources", "book.txt");
        BookText = File.ReadAllText(sourcePath);
        
        BookLines = BookText
            .Split('\n')
            .Select(line => line.TrimEnd('\r'))
            .ToList();
    }

    [Benchmark]
    public byte[] SingleTextInstanceWithMassiveContent()
    {
        return GeneratePdf(column =>
        {
            column.Item().Text(BookText).FontSize(10);
        });
    }

    [Benchmark]
    public byte[] ManyShortTextInstances()
    {
        return GeneratePdf(column =>
        {
            foreach (var line in BookLines)
                column.Item().Text(line).FontSize(10);
        });
    }

    private static byte[] GeneratePdf(Action<ColumnDescriptor> content)
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);

                page.Content().Column(column =>
                {
                    column.Spacing(5);
                    content(column);
                });
            });
        })
        .GeneratePdf();
    }
}
