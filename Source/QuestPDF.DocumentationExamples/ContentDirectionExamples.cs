using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ContentDirectionExamples
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
                        .ContentFromLeftToRight()
                        .Row(row =>
                        {
                            row.Spacing(5);
    
                            row.AutoItem().Height(50).Width(50).Background(Colors.Red.Lighten1);
                            row.AutoItem().Height(50).Width(50).Background(Colors.Green.Lighten1);
                            row.AutoItem().Height(50).Width(75).Background(Colors.Blue.Lighten1);
                        });
                });
            })
            .GenerateImages(x => "content-direction-ltr.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void RightToLeftExample()
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
                        .ContentFromRightToLeft()
                        .Row(row =>
                        {
                            row.Spacing(5);
    
                            row.AutoItem().Height(50).Width(50).Background(Colors.Red.Lighten1);
                            row.AutoItem().Height(50).Width(50).Background(Colors.Green.Lighten1);
                            row.AutoItem().Height(50).Width(75).Background(Colors.Blue.Lighten1);
                        });
                });
            })
            .GenerateImages(x => "content-direction-rtl.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}