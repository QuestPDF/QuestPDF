using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class BorderExamples
    {
        [Test]
        public void Border_Simple()
        {
            RenderingTest
                .Create()
                .PageSize(200, 150)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        
                        .BorderLeft(6)
                        .BorderTop(9)
                        .BorderRight(12)
                        .BorderBottom(15)
                        .BorderColor(Colors.Green.Darken3)
                        
                        .Background(Colors.Grey.Lighten2)
                        
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("Text")
                        .FontSize(20);
                });
        }
        
        [Test]
        public void Border_DifferentColors()
        {
            RenderingTest
                .Create()
                .PageSize(200, 150)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        
                        .BorderTop(5)
                        .BorderColor(Colors.Blue.Darken1)
                        
                        .Container()
                        
                        .BorderBottom(5)
                        .BorderColor(Colors.Green.Darken1)
                        
                        .Background(Colors.Grey.Lighten2)
                        
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("Text")
                        .FontSize(20);
                });
        }
    }
}