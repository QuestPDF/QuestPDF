using NUnit.Framework;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    [TestFixture]
    public class BarcodeExamples
    {
        [Test]
        public void Barcode()
        {
            RenderingTest
                .Create()
                .PageSize(300, 300)
                .Render(container =>
                {
                    container
                        .Background("#FFF")
                        .Padding(25)
                        .Stack(stack =>
                        {
                            stack.Item().Border(1).Background(Colors.Grey.Lighten3).Padding(5).Text("Barcode Example");
                            stack.Item().Border(1).Padding(5).AlignCenter().Text("*123456789*", TextStyle.Default.FontType("CarolinaBar-Demo-25E2").Size(20));
                        });
                });
        }
    }
}
