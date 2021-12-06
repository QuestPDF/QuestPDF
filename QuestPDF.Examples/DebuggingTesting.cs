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
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .Width(100)
                        .Background(Colors.Grey.Lighten3)
                        .DebugPointer("Example debug pointer")
                        .Stack(x =>
                        {
                            x.Item().Text("Test");
                            x.Item().Width(150);
                        });
                });
        }
        
        [Test]
        public void Simple()
        {
            RenderingTest
                .Create()
                .PageSize(500, 360)
                .Render(container =>
                {
                    container
                        .Padding(10)
                        .Width(100)
                        .Background(Colors.Grey.Lighten3)
                        .Width(150)
                        .Text("Test");
                });
        }
        
        [Test]
        public void DebugPointer()
        {
            RenderingTest
                .Create()
                .PageSize(500, 360)
                .Render(container =>
                {
                    container
                        .Width(100)
                        .DebugPointer("Example debug pointer")
                        .Width(150);
                });
        }
    }
}