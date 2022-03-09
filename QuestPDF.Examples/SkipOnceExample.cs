using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class SkipOnceExample
    {
        [Test]
        public void SkipOnce()
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
        
                        page.Header().Column(column =>
                        {
                            column.Item().ShowOnce().Text("This header is visible on the first page.");
                            column.Item().SkipOnce().Text("This header is visible on the second page and all following.");
                        });
                        
                        page.Content()
                            .PaddingVertical(10)
                            .Text(Placeholders.Paragraphs())
                            .Color(Colors.Grey.Medium);
                        
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