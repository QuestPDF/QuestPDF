using System.IO;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class BarCode
    {
        [Test]
        public void Example()
        {
            FontManager.RegisterFontType("LibreBarcode39", File.OpenRead("LibreBarcode39-Regular.ttf"));
            
            RenderingTest
                .Create()
                .PageSize(400, 100)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background(Colors.White)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("*QuestPDF*", TextStyle.Default.FontType("LibreBarcode39").Size(64));
                });
        }
    }
}