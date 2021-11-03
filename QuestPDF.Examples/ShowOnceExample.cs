using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class ShowOnceExample
    {
        [Test]
        public void ShowOnce()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .ShowResults()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(20);
                        page.Size(PageSizes.A7.Landscape());
                        page.Background(Colors.White);

                        page.Header().Text("With show once", TextStyle.Default.SemiBold());
                        
                        page.Content().PaddingVertical(5).Row(row =>
                        {
                            row.RelativeColumn()
                                .Background(Colors.Grey.Lighten2)
                                .Border(1)
                                .Padding(5)
                                .ShowOnce()
                                .Text(Placeholders.Label());
                            
                            row.RelativeColumn(2)
                                .Border(1)
                                .Padding(5)
                                .Text(Placeholders.Paragraph());
                        });
                        
                        page.Footer().Text(text =>
                        {
                            text.Span("Page ");
                            text.CurrentPageNumber();
                            text.Span(" out of ");
                            text.TotalPages();
                        });
                    });
                });
        }
    }
}