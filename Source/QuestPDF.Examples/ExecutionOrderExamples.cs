using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class ExecutionOrderExamples
    {
        [Test]
        public void Example()
        {
            RenderingTest
                .Create()
                .PageSize(400, 300)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .DefaultTextStyle(TextStyle.Default.Size(18))
                        .Padding(25)
                        .Row(row =>
                        {
                            row.Spacing(25);

                            row.RelativeItem()
                                .Border(1)
                                .Padding(15)
                                .Background(Colors.Grey.Lighten2)
                                .Text("Lorem ipsum");
                            
                            row.RelativeItem()
                                .Border(1)
                                .Background(Colors.Grey.Lighten2)
                                .Padding(15)
                                .Text("dolor sit amet");
                        });
                });
        }
        
        [Test]
        public void Example2()
        {
            RenderingTest
                .Create()
                .PageSize(200, 200)
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Border(2)
                        .Width(150)
                        .Height(150)

                        .Background(Colors.Blue.Lighten2)
                        .PaddingTop(50)

                        .Background(Colors.Green.Lighten2)
                        .PaddingRight(50)

                        .Background(Colors.Red.Lighten2)
                        .PaddingBottom(50)

                        .Background(Colors.Amber.Lighten2)
                        .PaddingLeft(50)

                        .Background(Colors.Grey.Lighten2);
                });
        }
    }
}