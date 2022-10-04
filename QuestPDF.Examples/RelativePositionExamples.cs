using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class RelativePositionExamples
    {
        [Test]
        public void ItemTypes()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(500, 500)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(100)
                        .Background(Colors.Grey.Lighten2)
                        .RelativePositionVertical(0.5f, -0.5f)
                        .RelativePositionHorizontal(1f, -0.5f)
                        .RelativeWidth(0.4f)
                        .RelativeHeight(0.6f)
                        .Background(Colors.Grey.Darken2);
                });
        }
    }
}