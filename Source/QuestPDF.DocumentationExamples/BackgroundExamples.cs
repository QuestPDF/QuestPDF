using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class BackgroundExamples
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

                    var colors = new[]
                    {
                        Colors.LightBlue.Darken4,
                        Colors.LightBlue.Darken3,
                        Colors.LightBlue.Darken2,
                        Colors.LightBlue.Darken1,
    
                        Colors.LightBlue.Medium,
    
                        Colors.LightBlue.Lighten1,
                        Colors.LightBlue.Lighten2,
                        Colors.LightBlue.Lighten3,
                        Colors.LightBlue.Lighten4,
                        Colors.LightBlue.Lighten5,
    
                        Colors.LightBlue.Accent1,
                        Colors.LightBlue.Accent2,
                        Colors.LightBlue.Accent3,
                        Colors.LightBlue.Accent4,
                    };
                    
                    page.Content()
                        .Height(100)
                        .Width(280)
                        .Row(row =>
                        {
                            foreach (var color in colors)
                                row.RelativeItem().Background(color);
                        });
                });
            })
            .GenerateImages(x => "background.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}