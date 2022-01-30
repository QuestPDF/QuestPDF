using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    static class SimpleExtension
    {
        private static IContainer Cell(this IContainer container, bool dark)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Medium)
                .Background(dark ? Colors.Grey.Lighten3 : Colors.White)
                .Padding(5);
        }
        
        public static void LabelCell(this IContainer container, string text) => container.Cell(true).Text(text, TextStyle.Default.SemiBold());
        public static IContainer ValueCell(this IContainer container) => container.Cell(false);
        public static void ValueCell(this IContainer container, string text) => container.ValueCell().Text(text, TextStyle.Default);
    }
    
    public class FrameExample
    {
        [Test]
        public void Frame()
        {
            RenderingTest
                .Create()
                .PageSize(550, 400)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(25)
                        .Column(column =>
                        {
                            for(var i = 1; i <= 4; i++)
                            {
                                column.Item().Row(row =>
                                {
                                    row.RelativeItem(2).LabelCell(Placeholders.Label());
                                    row.RelativeItem(3).ValueCell().Text(Placeholders.Paragraph());
                                });
                            }
                        });
                });
        }
    }
}