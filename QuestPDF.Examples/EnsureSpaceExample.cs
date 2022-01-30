using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class EnsureSpaceExample
    {
        [Test]
        public void EnsureSpaceWith()
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
                        
                        page.Header().Text("With ensure space", TextStyle.Default.SemiBold());
                        
                        page.Content().Column(column =>
                        {
                            column
                                .Item()
                                .ExtendHorizontal()
                                .Height(75)
                                .Background(Colors.Grey.Lighten2);
                            
                            column
                                .Item()
                                .EnsureSpace(100)
                                .Text(Placeholders.LoremIpsum());
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