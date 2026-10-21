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
                        .Column(column =>
                        {
                            column.Spacing(5);

                            foreach (var itemsInRow in new[] { 3, 3, 2 })
                            {
                                column.Item().Row(row =>
                                {
                                    row.Spacing(5);

                                    foreach (var _ in Enumerable.Range(0, itemsInRow))
                                        row.RelativeItem().Height(50).Placeholder();

                                    if (itemsInRow < 3)
                                        row.RelativeItem(3 - itemsInRow);
                                });
                            }
                        });
                });
            })
            .GenerateImages(x => "debug-area.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 216 });
    }
}