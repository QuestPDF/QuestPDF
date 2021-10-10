using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class InlinedExamples
    {
        [Test]
        public void Inlined()
        {
            RenderingTest
                .Create()
                .PageSize(800, 575)
                .FileName()
                .ProduceImages()
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Border(1)
                        .Background(Colors.Grey.Lighten2)
                        .Inlined(inlined =>
                        {
                            inlined.Spacing(25);
                            
                            inlined.AlignCenter();
                            inlined.BaselineMiddle();

                            var random = new Random();
                            
                            foreach (var _ in Enumerable.Range(0, 50))
                            {
                                inlined
                                    .Item()
                                    .Border(1)
                                    .Width(random.Next(1, 5) * 25)
                                    .Height(random.Next(1, 5) * 25)
                                    .Background(Placeholders.BackgroundColor());
                            }
                        });
                });
        }
    }
}