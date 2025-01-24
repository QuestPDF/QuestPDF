using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class PaddingExamples
{
    [Test]
    public void SimpleExample()
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
                        .Width(250)
                        .PaddingVertical(10)
                        .PaddingLeft(20)
                        .PaddingRight(40)
                        .Background(Colors.Grey.Lighten2)
                        .Text("Sample text");
                });
            })
            .GenerateImages(x => "padding-simple.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void NegativeExample()
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
                        .Width(250)
                        .Padding(50)
                        .Background(Colors.Grey.Lighten2)
                        .PaddingHorizontal(-25)
                        .Text("Sample text with negative padding");
                });
            })
            .GenerateImages(x => "padding-negative.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}