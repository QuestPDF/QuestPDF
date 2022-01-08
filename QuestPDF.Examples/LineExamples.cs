using System.IO;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class LineExamples
    {
        [Test]
        public void LineHorizontal()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A5)
                .ProducePdf()
                .ShowResults()
                .Render(container => 
                {
                    container.Padding(25).Column(column =>
                    {
                        column.Item().Text("Above text");
                        column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Blue.Accent1);
                        column.Item().Text("Below text");
                    });
                });
        }
        
        [Test]
        public void LineVertical()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A5)
                .ProducePdf()
                .ShowResults()
                .Render(container => 
                {
                    container.Padding(25).Row(row =>
                    {
                        row.Spacing(25);
                        // row.AutoItem().Text("Above text");
                        // row.AutoItem().LineVertical(1).LineColor(Colors.Blue.Accent1);
                        // row.AutoItem().Text("Below text");
                    });
                });
        }
    }
}