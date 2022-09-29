using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class RightToLeftExamples
    {
        [Test]
        public void Unconstrained()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .AlignCenter()
                        .AlignMiddle()
                        .ContentFromRightToLeft()
                        .Unconstrained()
                        .Background(Colors.Red.Medium)
                        .Height(200)
                        .Width(200);
                });
        }
        
        [Test]
        public void Row()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .ContentFromRightToLeft()
                        .Border(1)
                        .MinimalBox()
                        .Row(row =>
                        {
                            row.ConstantItem(200).Background(Colors.Red.Lighten2).Height(200);
                            row.ConstantItem(150).Background(Colors.Green.Lighten2).Height(200);
                            row.ConstantItem(100).Background(Colors.Blue.Lighten2).Height(200);
                        });
                });
        }
    }
}