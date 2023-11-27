using System;
using System.Linq;
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
                        page.PageColor(Colors.White);

                        page.Header().Text("With show once").SemiBold();
                        
                        page.Content().PaddingVertical(5).Row(row =>
                        {
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten2)
                                .Border(1)
                                .Padding(5)
                                .ShowOnce()
                                .Text(Placeholders.Label());
                            
                            row.RelativeItem(2)
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
        
        [Test]
        public void ShowIf()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .ShowResults()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Margin(20);
                        page.Size(PageSizes.A4);
                        page.PageColor(Colors.White);

                        page.DefaultTextStyle(x => x.FontSize(20));
                        
                        page.Header().Text("Show when example").SemiBold();
                        
                        page.Content().Column(column =>
                        {
                            column.Spacing(10);

                            foreach (var s in Enumerable.Range(0, 10))
                            {
                                foreach (var i in Enumerable.Range(0, Random.Shared.Next(10, 50)))
                                {
                                    column
                                        .Item()
                                        .Height(40)
                                        .Width(150)
                                        .Background(Colors.Grey.Lighten3)
                                        .Text($"{s} - {i}");
                                }   
                                
                                column.Item().PageBreak();
                                column.Item().ShowIf(x => x.PageNumber % 2 == 0).PageBreak();
                            }
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