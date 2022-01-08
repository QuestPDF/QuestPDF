using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class ScaleToFitExamples
    {
        [Test]
        public void ScaleToFit()
        {
            RenderingTest
                .Create()
                .PageSize(PageSizes.A4)
                .ProducePdf()
                .ShowResults()
                .Render(container =>
                {
                    container.Padding(25).Column(column =>
                    {
                        var text = Placeholders.Paragraph();

                        foreach (var i in Enumerable.Range(0, 16))
                        {
                            column
                                .Item()
                                .MinimalBox()
                                .Border(1)
                                .Padding(5)
                                .Width(50 + i * 25)
                                .Height(25 + i * 5)
                                .ScaleToFit()
                                .Text(text);
                        }
                    });
                });
        }
    }
}