using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ScaleExamples
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
                        .Width(350)
                        .Padding(25)
                        .Column(column =>
                        {
                            column.Spacing(10);
                            
                            var scales = new[] { 0.75f, 1f, 1.25f, 1.5f };

                            foreach (var scale in scales)
                            {
                                column
                                    .Item()
                                    .Background(Colors.Grey.Lighten3)
                                    .Scale(scale)
                                    .Padding(10)
                                    .Text($"Content scale: {scale}")
                                    .FontSize(20);
                            }
                        });
                });
            })
            .GenerateImages(x => "scale.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}