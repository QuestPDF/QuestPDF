using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class RelativeSizeExamples
    {
        [Test]
        public void ItemTypes()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(600, 600)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .AlignMiddle()
                        .AlignCenter()
                        .Width(400)
                        .Height(400)
                        .Background(Colors.Grey.Lighten2)
                        .AlignMiddle()
                        .AlignCenter()
                        .Container()
                        .AlignMiddle()
                        .AlignCenter()
                        .RelativeWidth(0.25f)
                        .RelativeHeight(0.5f)
                        .Background(Colors.Grey.Darken2);
                });
        }
    }
}