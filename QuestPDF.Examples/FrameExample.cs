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
                .Background(dark ? Colors.Grey.Lighten2 : Colors.White)
                .Padding(10);
        }
        
        public static void LabelCell(this IContainer container, string text) => container.Cell(true).Text(text, TextStyle.Default.Medium());
        public static IContainer ValueCell(this IContainer container) => container.Cell(false);
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
                        .Stack(stack =>
                        {
                            for(var i=1; i<=4; i++)
                            {
                                stack.Item().Row(row =>
                                {
                                    row.RelativeColumn(2).LabelCell(Placeholders.Label());
                                    row.RelativeColumn(3).ValueCell().Text(Placeholders.Paragraph());
                                });
                            }
                        });
                });
        }
    }
}