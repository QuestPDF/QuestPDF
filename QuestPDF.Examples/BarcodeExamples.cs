using System.IO;
using NUnit.Framework;
using QuestPDF.Drawing;
using QuestPDF.Examples.Engine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
{
    public class BarcodeExamples
    {
        [Test]
        public void Example()
        {
            FontManager.RegisterFont(File.OpenRead("LibreBarcode39-Regular.ttf"));
            
            RenderingTest
                .Create()
                .PageSize(400, 200)
                .ShowResults()
                .Render(container =>
                {
                    container
                        .Background(Colors.White)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("*QuestPDF*")
                        .FontFamily("Libre Barcode 39")
                        .FontSize(64);
                });
        }
    }
}