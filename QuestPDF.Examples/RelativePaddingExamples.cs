using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class RelativePaddingExamples
    {
        [Test]
        public void ItemTypes()
        {
            RenderingTest
                .Create()
                .ProduceImages()
                .PageSize(250, 250)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Width(250)
                        .Height(250)
                            
                        .Padding(50)
                        .Background(Colors.Grey.Lighten2)
                        
                        .RelativePaddingLeft(0.1f)
                        .RelativePaddingTop(0.2f)
                        .RelativePaddingRight(0.3f)
                        .RelativePaddingBottom(0.4f)
                        
                        .Background(Colors.Grey.Darken2);
                });
        }
    }
}