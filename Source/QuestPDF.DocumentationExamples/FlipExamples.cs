using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class FlipExamples
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
                        .Height(350)
                        .Padding(20)
                        .Grid(grid =>
                        {
                            grid.Columns(2);
                            grid.Spacing(10);
    
                            foreach (var turns in Enumerable.Range(0, 4))
                            {
                                grid.Item()
                                    .Width(150)
                                    .Height(150)
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(10)
                                    .Element(element =>
                                    {
                                        if (turns == 1 || turns == 2)
                                            element = element.FlipHorizontal();

                                        if (turns == 2 || turns == 3)
                                            element = element.FlipVertical();
                
                                        return element;
                                    })
                                    .Shrink()
                                    .Background(Colors.White)
                                    .Padding(10)
                                    .Text($"Flipped {turns}").FontSize(16);
                            }
                        });
                });
            })
            .GenerateImages(x => "flip.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}