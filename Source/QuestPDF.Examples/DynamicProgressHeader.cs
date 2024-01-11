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
    public class ProgressHeader : IDynamicComponent
    {
        public DynamicComponentComposeResult Compose(DynamicContext context)
        {
            var content = context.CreateElement(container =>
            {
                var width = context.AvailableSize.Width * context.PageNumber / context.TotalPages;
                
                container
                    .Background(Colors.Blue.Lighten2)
                    .Height(25)
                    .Width(width)
                    .Background(Colors.Blue.Darken1);
            });

            return new DynamicComponentComposeResult
            {
                Content = content,
                HasMoreContent = false
            };
        }
    }
    
    public static class DynamicProgressHeader
    {
        [Test]
        public static void Dynamic()
        {
            RenderingTest
                .Create()
                .ShowResults()
                .MaxPages(100)
                .ProducePdf()
                .RenderDocument(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A6);
                        page.Margin(1, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(20));

                        page.Header().Dynamic(new ProgressHeader());
                        
                        page.Content().Column(column =>
                        {
                            foreach (var i in Enumerable.Range(0, 100))
                                column.Item().PaddingTop(25).Background(Colors.Grey.Lighten2).Height(50);
                        });

                        page.Footer().AlignCenter().Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(20));
                            
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                    });
                });
        }
    }
}