using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ColorsExamples
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
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Content() 
                        .Width(175)
                        .Padding(20)
                        .Border(1)
                        .BorderColor("#03A9F4")
                        .Background(Colors.LightBlue.Lighten5)
                        .Padding(20)
                        .Text("Blue text")
                        .Bold()
                        .FontColor(Colors.LightBlue.Darken4)
                        .Underline()
                        .DecorationWavy()
                        .DecorationColor(0xFF0000);
                });
            })
            .GenerateImages(x => "colors.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}