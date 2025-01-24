using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class BorderExamples
{
    [Test]
    public void SimpleExample()
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
                        .Width(150)
                        .Padding(25)  
                        .BorderLeft(4)
                        .BorderTop(6)
                        .BorderRight(8) 
                        .BorderBottom(10)
                        .BorderColor(Colors.LightBlue.Darken3)
                        .Background(Colors.Grey.Lighten3)
                        .Padding(25) 
                        .Text("Text");
                });
            })
            .GenerateImages(x => "border-simple.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void ManyExample()
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
                        .Width(150)
                        .Padding(25)

                        .BorderTop(5)
                        .BorderColor(Colors.LightGreen.Darken2)

                        .Container()

                        .BorderBottom(10)
                        .BorderColor(Colors.LightBlue.Darken2)
                        
                        .Background(Colors.Grey.Lighten3)
                        .Padding(25)
                        .Text("Text")
                        .FontSize(20);
                });
            })
            .GenerateImages(x => "border-many.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}