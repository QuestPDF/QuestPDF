using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class PageBackgroundForeground
    {
        [Test]
        public void Frame()
        {
            RenderingTest
                .Create()
                .PageSize(550, 400)
                .ProducePdf()
                .ShowResults()
                .RenderDocument(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(1, Unit.Inch);
                        page.DefaultTextStyle(TextStyle.Default.FontSize(16));

                        page.Foreground()
                            .AlignMiddle()
                            .AlignCenter()
                            .Text("Watermark")
                            .FontSize(64)
                            .FontColor(Colors.Blue.Lighten4);
                        
                        page.Header().Text("Background and foreground").Bold().FontColor(Colors.Blue.Medium).FontSize(24);
                        
                        page.Content().PaddingVertical(25).Column(column =>
                        {
                            column.Spacing(25);

                            foreach (var i in Enumerable.Range(0, 100))
                                column.Item().Background(Colors.Grey.Lighten2).Height(75);
                        });
                        
                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                            });
                    });
                });
        }
    }
}