using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class AlignmentExamples
    {
        [Test]
        public void Example()
        {
            RenderingTest
                .Create()
                .PageSize(400, 200)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Border(1)
                        .AlignBottom()
                        .Background(Colors.Grey.Lighten1)
                        .Text("Test");
                });
        }
    }
}