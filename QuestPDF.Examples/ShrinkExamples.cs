using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class ShrinkExamples
    {
        [Test]
        public void Shrink_Without()
        {
            RenderingTest
                .Create()
                .PageSize(300, 200)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Border(2)
                        .Background(Colors.Grey.Lighten2)
                        .Padding(20)
                        .Text("This is test.")
                        .FontSize(20);
                });
        }
        
        [Test]
        public void Shrink_Horizontal()
        {
            RenderingTest
                .Create()
                .PageSize(300, 200)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Border(2)
                        .ShrinkHorizontal()
                        .Background(Colors.Grey.Lighten2)
                        .Padding(20)
                        .Text("This is test.")
                        .FontSize(20);
                });
        }
        
        [Test]
        public void Shrink_Vertical()
        {
            RenderingTest
                .Create()
                .PageSize(300, 200)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Border(2)
                        .ShrinkVertical()
                        .Background(Colors.Grey.Lighten2)
                        .Padding(20)
                        .Text("This is test.")
                        .FontSize(20);
                });
        }
        
        [Test]
        public void Shrink_Both()
        {
            RenderingTest
                .Create()
                .PageSize(300, 200)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(20)
                        .Border(2)
                        .Shrink()
                        .Background(Colors.Grey.Lighten2)
                        .Padding(20)
                        .Text("This is test.")
                        .FontSize(20);
                });
        }
    }
}