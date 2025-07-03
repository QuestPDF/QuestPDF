using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class BackgroundExamples
{
    [Test]
    public void SolidColor()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.PageColor(Colors.White);
                    page.Margin(25);

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
                        .Height(150)
                        .Width(420)
                        .Row(row =>
                        {
                            foreach (var color in colors)
                                row.RelativeItem().Background(color);
                        });
                });
            })
            .GenerateImages(x => "background-solid.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void Gradient() 
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(350, 0));
                    page.MaxSize(new PageSize(350, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.PageColor(Colors.White);
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(25);
 
                            column.Item()
                                .BackgroundLinearGradient(0, [Colors.Red.Lighten2, Colors.Blue.Lighten2])
                                .AspectRatio(2);

                            column.Item()
                                .BackgroundLinearGradient(45, [Colors.Green.Lighten2, Colors.LightGreen.Lighten2, Colors.Yellow.Lighten2])
                                .AspectRatio(2);
                            
                            column.Item()
                                .BackgroundLinearGradient(90, [Colors.Yellow.Lighten2, Colors.Amber.Lighten2, Colors.Orange.Lighten2])
                                .AspectRatio(2);
                        });
                });
            })
            .GenerateImages(x => "background-gradient.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void RoundedCorners() 
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {   
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.PageColor(Colors.White);
                    page.Margin(25);

                    page.Content()
                        .Shrink()
                        .Background(Colors.Grey.Lighten2)
                        .CornerRadius(25)
                        .Padding(25)
                        .Text("Content with rounded corners");
                });
            })
            .GenerateImages(x => "background-rounded-corners.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}