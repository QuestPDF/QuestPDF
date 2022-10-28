using System.Linq;
using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace QuestPDF.Examples
{
    public class RepeatContentExamples
    {
        [Test]
        public void ItemTypes()
        {
            RenderingTest
                .Create()
                .ProducePdf()
                .PageSize(PageSizes.A4)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Padding(25)
                        .Decoration(decoration =>
                        {
                            decoration.Before().Text("Test").FontSize(22);
                            
                            decoration.Content().Column(column =>
                            {
                                column.Spacing(20);

                                foreach (var _ in Enumerable.Range(0, 10))
                                    column.Item().Background(Colors.Grey.Medium).ExtendHorizontal().Height(80);
                            }); 
                        });
                });
        }
    }
}