using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class DebuggingTesting
    {
        [Test]
        public void Stack()
        {
            RenderingTest
                .Create()
                .PageSize(500, 360)
                .FileName()
                .Render(container =>
                {
                    container
                        .Background(Colors.White)
                        .Padding(15)
                        .Grid(grid =>
                        {
                            grid.Spacing(15);
                    
                            grid.Item().Background(Colors.Grey.Medium).Height(50);
                            grid.Item().Background(Colors.Grey.Lighten1).Height(1000); // <-- problem
                            grid.Item().Background(Colors.Grey.Lighten2).Height(150);
                        });
                });
        }
    }
}