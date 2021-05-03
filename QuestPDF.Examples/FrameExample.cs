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
                .Background(dark ? "#EEE" : "#FFF")
                .Padding(10);
        }
        
        public static IContainer LabelCell(this IContainer container) => container.Cell(true);
        public static IContainer ValueCell(this IContainer container) => container.Cell(false);
    }
    
    public class FrameExample: ExampleTestBase
    {
        [ImageSize(550, 400)]
        [ShowResult]
        public void Frame(IContainer container)
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
                            row.RelativeColumn(2).LabelCell().Text(Placeholders.Label());
                            row.RelativeColumn(3).ValueCell().Text(Placeholders.Paragraph());
                        });
                    }
                });
        }
    }
}