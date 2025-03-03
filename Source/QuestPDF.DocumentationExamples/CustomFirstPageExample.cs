using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class CustomFirstPageExample
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(20));
    
                    page.Header().Column(column =>
                    {
                        column.Item().ShowOnce().Background(Colors.Blue.Lighten2).Height(80);
                        column.Item().SkipOnce().Background(Colors.Green.Lighten2).Height(60);
                    });
    
                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        column.Spacing(20);

                        foreach (var _ in Enumerable.Range(0, 20))
                            column.Item().Background(Colors.Grey.Lighten3).Height(40);
                    });
    
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            })
            .GeneratePdf("example-custom-first-page.pdf");
    }
}