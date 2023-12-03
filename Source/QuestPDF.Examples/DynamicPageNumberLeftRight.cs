using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Elements;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class FooterWithAlternatingAlignment : IDynamicComponent
    {
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var content = context.CreateElement(element =>
            {
                element
                    .Element(x => context.PageNumber % 2 == 0 ? x.AlignLeft() : x.AlignRight())
                    .Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
            });
            
            return new DynamicComponentComposeResult()
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }
    
    public static class DynamicPageNumberLeftRightExamples
    {
        [Test]
        public static void Dynamic()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A5)
                .MaxPages(100)
                .ShowResults()
                .ProducePdf()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A6);
                        page.PageColor(Colors.White);
                        page.Margin(1, Unit.Centimetre);
                        page.DefaultTextStyle(x => x.FontSize(18));

                        page.Content().Column(column =>
                        {
                            foreach (var i in Enumerable.Range(0, 50))
                                column.Item().PaddingTop(25).Background(Colors.Grey.Lighten2).Height(50);
                        });
                        
                        page.Footer().Dynamic(new FooterWithAlternatingAlignment());
                    });
                });
        }
    }
}