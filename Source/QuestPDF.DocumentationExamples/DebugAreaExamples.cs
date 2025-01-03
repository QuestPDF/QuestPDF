using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class DebugAreaExamples
{
    [Test]
    public void LeftToRightExample()
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
                        .Height(250)
                        .Padding(25)
                        .DebugArea("Grid example", Colors.Blue.Medium)
                        .Grid(grid =>
                        {
                            grid.Columns(3);
                            grid.Spacing(5);

                            foreach (var _ in Enumerable.Range(0, 8))
                                grid.Item().Height(50).Placeholder();
                        });
                });
            })
            .GenerateImages(x => "debug-area.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 216 });
    }
}