using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class TranslateExamples
{
    [Test]
    public void Example()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(400, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Content()
                        .Padding(50)
                        .Background(Colors.Blue.Lighten3)
                        .TranslateX(25)
                        .TranslateY(25)
                        .Border(4)
                        .BorderColor(Colors.Blue.Darken2)
                        .Padding(50)
                        .Text("Moved content")
                        .FontSize(25);
                });
            })
            .GenerateImages(x => "translate.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}