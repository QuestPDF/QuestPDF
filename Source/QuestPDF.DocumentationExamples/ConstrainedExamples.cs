using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ConstrainedExamples
{
    [Test]
    public void WidthExample()
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
                        .Padding(25)
                        .Column(column =>
                        {
                            column.Spacing(25);
                            
                            column.Item()
                                .MinWidth(200)
                                .Background(Colors.Grey.Lighten3)
                                .Text("Lorem ipsum");
                            
                            column.Item()
                                .MaxWidth(100)
                                .Background(Colors.Grey.Lighten3)
                                .Text("dolor sit amet");
                        });
                });
            })
            .GenerateImages(x => "width.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void HeightExample()
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
                        .Padding(25)
                        .Height(100)
                        .AspectRatio(2f, AspectRatioOption.FitHeight)
                        .Background(Colors.Grey.Lighten1);
                });
            })
            .GenerateImages(x => "height.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}