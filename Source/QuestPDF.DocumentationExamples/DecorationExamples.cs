using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class DecorationExamples
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
                        .MaxHeight(300)
                        .MaxWidth(300)
                        .Decoration(decoration =>
                        {
                            decoration
                                .Before()
                                .Background(Colors.Grey.Medium)
                                .Padding(10)
                                .Text("Notes").FontColor("#FFF").Bold();

                            decoration
                                .Content()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(10)
                                .Text(Helpers.Placeholders.LoremIpsum());
                        });
                });
            })
            .GenerateImages(x => $"decoration-{x}.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}