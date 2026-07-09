using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class MergingDocumentsExamples
{
    [Test]
    public async Task UseOriginalPageNumbersExample()
    {
        Document
            .Merge(
                GenerateReport("Short Document 1", 5),
                GenerateReport("Medium Document 2", 10),
                GenerateReport("Long Document 3", 15))
            .UseOriginalPageNumbers()
            .GeneratePdf("merged.pdf");
    }
    
    [Test]
    public async Task UseContinuousPageNumbersExample()
    {
        Document
            .Merge(
                GenerateReport("Short Document 1", 5),
                GenerateReport("Medium Document 2", 10),
                GenerateReport("Long Document 3", 15))
            .UseContinuousPageNumbers()
            .GeneratePdf("merged.pdf");
    }
    
    
    #region Example document

    private static Document GenerateReport(string title, int itemsCount)
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(0.5f, Unit.Inch);
            
                page.Header()
                    .Text(title)
                    .Bold()
                    .FontSize(24)
                    .FontColor(Colors.Blue.Accent2);
            
                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        foreach (var i in Enumerable.Range(0, itemsCount))
                        {
                            column
                                .Item()
                                .Width(200)
                                .Height(50)
                                .Background(Colors.Grey.Lighten3)
                                .AlignMiddle()
                                .AlignCenter()
                                .Text($"Item {i}")
                                .FontSize(16);
                        }
                    });
            
                page.Footer()
                    .AlignCenter()
                    .PaddingVertical(20)
                    .Text(text =>
                    {
                        text.DefaultTextStyle(TextStyle.Default.FontSize(16));
                    
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        });
    }

    #endregion
}