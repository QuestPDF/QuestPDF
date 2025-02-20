using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class LineExamples
{
    [Test]
    public void VerticalLineExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Row(row =>
                        {
                            row.AutoItem().Text("Text on the left");
                            
                            row.AutoItem()
                                .PaddingHorizontal(15)
                                .LineVertical(3)
                                .LineColor(Colors.Blue.Medium);
                            
                            row.AutoItem().Text("Text on the right");
                        });
                });
            })
            .GenerateImages(x => "line-vertical.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void HorizontalLineExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Item().Text("Text above the line");
                            
                            column.Item()
                                .PaddingVertical(10)
                                .LineHorizontal(2)
                                .LineColor(Colors.Blue.Medium);
                            
                            column.Item().Text("Text below the line");
                        });
                });
            })
            .GenerateImages(x => "line-horizontal.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}