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
                    page.PageColor(Colors.White);

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
                    page.PageColor(Colors.White);

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
    
    [Test]
    public void Thickness()
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
                    page.PageColor(Colors.White);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(20);
    
                            foreach (var thickness in new[] { 1, 2, 4, 8 })
                            {
                                column.Item()
                                    .Width(200)
                                    .LineHorizontal(thickness);
                            }
                        });
                });
            })
            .GenerateImages(x => "line-thickness.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }

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
                    page.Margin(25);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Column(column =>
                        {
                            var colors = new[]
                            {
                                Colors.Red.Medium,
                                Colors.Green.Medium,
                                Colors.Blue.Medium,
                            };
                            
                            column.Spacing(20);
                
                            foreach (var color in colors)
                            {
                                column.Item()
                                    .Width(200)
                                    .LineHorizontal(5)
                                    .LineColor(color);
                            }
                        });
                });
            })
            .GenerateImages(x => "line-color-solid.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Gradient()
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
                    page.PageColor(Colors.White);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item()
                                .Width(200)
                                .LineHorizontal(5)
                                .LineGradient([Colors.Red.Medium, Colors.Orange.Medium]);
                
                            column.Item()
                                .Width(200)
                                .LineHorizontal(5)
                                .LineGradient([Colors.Orange.Medium, Colors.Yellow.Medium, Colors.Lime.Medium]);
                
                            column.Item()
                                .Width(200)
                                .LineHorizontal(5)
                                .LineGradient([Colors.Blue.Lighten2, Colors.LightBlue.Lighten1, Colors.Cyan.Medium, Colors.Teal.Darken1, Colors.Green.Darken2]);
                        });
                });
            })
            .GenerateImages(x => "line-color-gradient.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void DashPattern()
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
                    page.PageColor(Colors.White);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(20);

                            column.Item()
                                .Width(200)
                                .LineHorizontal(5)
                                .LineDashPattern([4f, 4f]);
                
                            column.Item()
                                .Width(200)
                                .LineHorizontal(5)
                                .LineDashPattern([12f, 12f]);
                
                            column.Item()
                                .Width(200)
                                .LineHorizontal(5)
                                .LineDashPattern([4f, 4f, 12f, 4f]);
                        });
                });
            })
            .GenerateImages(x => "line-dash-pattern.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Complex()
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
                    page.PageColor(Colors.White);

                    page.Content()
                        .Width(300)
                        .LineHorizontal(8)
                        .LineDashPattern([4, 4, 8, 8, 12, 12])
                        .LineGradient([Colors.Red.Medium, Colors.Orange.Medium, Colors.Yellow.Medium]);
                });
            })
            .GenerateImages(x => "line-example.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}