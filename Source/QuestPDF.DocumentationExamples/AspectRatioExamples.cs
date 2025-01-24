using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class AspectRatioExamples
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
                        .Width(300)
                        .Height(300)
                        .AspectRatio(3f/4f, AspectRatioOption.FitArea)
                        .Background(Colors.Grey.Lighten2)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text("3:4 Content Area");
                });
            })
            .GenerateImages(x => "aspect-ratio.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}