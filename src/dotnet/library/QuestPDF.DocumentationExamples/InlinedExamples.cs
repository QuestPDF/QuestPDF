using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class InlinedExamples
{
    [Test]
    public void SimpleExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.ContinuousSize(450);

                    page.Content()
                        .Background(Colors.Grey.Lighten3)
                        .Padding(25)
                        .Border(1)
                        .Background(Colors.White)
                        .Inlined(inlined =>
                        {
                            inlined.Spacing(25);
                            inlined.BaselineMiddle();
                            inlined.AlignCenter();
                            
                            foreach (var _ in Enumerable.Range(0, 15))
                                inlined.Item().Element(RandomBlock);
                        });
                });
            })
            .GenerateImages(x => "inlined.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void SpacingExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.ContinuousSize(450);

                    page.Content()
                        .Background(Colors.Grey.Lighten3)
                        .Padding(25)
                        .Border(1)
                        .Background(Colors.White)
                        .Inlined(inlined =>
                        {
                            inlined.VerticalSpacing(15);
                            inlined.HorizontalSpacing(30);
                            
                            foreach (var _ in Enumerable.Range(0, 10))
                                inlined.Item().Element(RandomBlock);
                        });
                });
            })
            .GenerateImages(x => "inlined-spacing.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    private void RandomBlock(IContainer container)
    {
        container
            .Width(Random.Shared.Next(1, 4) * 25)
            .Height(Random.Shared.Next(1, 4) * 25)
            .Border(1)
            .BorderColor(Colors.Grey.Darken2)
            .Background(Placeholders.BackgroundColor());
    }
}