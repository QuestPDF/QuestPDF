using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class ShadowExamples
{
    [Test]
    public void Simple()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(50);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Border(1, Colors.Black)
                        .Shadow(new BoxShadowStyle
                        {
                            Color = Colors.Grey.Medium, 
                            Blur = 5, 
                            Spread = 5,
                            OffsetX = 5, 
                            OffsetY = 5
                        })
                        .Background(Colors.White)
                        .Padding(15)
                        .Text("Important content");
                });
            })
            .GenerateImages(x => $"shadow-simple.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void OffsetX()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(50);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Row(row =>
                        {
                            row.Spacing(50);
                            
                            foreach (var offsetX in new[] { -10, 0, 10 })
                            {
                                row.ConstantItem(100)
                                    .AspectRatio(1)
                                    .Shadow(new BoxShadowStyle
                                    {
                                        Color = Colors.Grey.Darken1,
                                        Blur = 10,
                                        OffsetX = offsetX
                                    })
                                    .Background(Colors.White);
                            }
                        });
                });
            })
            .GenerateImages(x => $"shadow-offset-x.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void OffsetY()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(50);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Row(row =>
                        {
                            row.Spacing(50);
                            
                            foreach (var offsetY in new[] { -10, 0, 10 })
                            {
                                row.ConstantItem(100)
                                    .AspectRatio(1)
                                    .Shadow(new BoxShadowStyle
                                    {
                                        Color = Colors.Grey.Darken2,
                                        Blur = 10,
                                        OffsetY = offsetY
                                    })
                                    .Background(Colors.White);
                            }
                        });
                });
            })
            .GenerateImages(x => $"shadow-offset-y.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Color()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(50);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Row(row =>
                        {
                            row.Spacing(50);
                            
                            var colors = new[]
                            {
                                Colors.Red.Darken2,
                                Colors.Green.Darken2,
                                Colors.Blue.Darken2
                            };
                            
                            foreach (var color in colors)
                            {
                                row.ConstantItem(100)
                                    .AspectRatio(1)
                                    .Shadow(new BoxShadowStyle
                                    {
                                        Color = color,
                                        Blur = 10
                                    })
                                    .Background(Colors.White);
                            }
                        });
                });
            })
            .GenerateImages(x => $"shadow-color.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Blur()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(50);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Row(row =>
                        {
                            row.Spacing(50);

                            foreach (var blur in new[] { 5, 10, 20 })
                            {
                                row.ConstantItem(100)
                                    .AspectRatio(1)
                                    .Shadow(new BoxShadowStyle
                                    {
                                        Color = Colors.Grey.Darken1,
                                        Blur = blur
                                    })
                                    .Background(Colors.White);
                            }
                        });
                });
            })
            .GenerateImages(x => $"shadow-blur.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Spread()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(50);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Row(row =>
                        {
                            row.Spacing(50);

                            foreach (var spread in new[] { 0, 5, 10 })
                            {
                                row.ConstantItem(100)
                                    .AspectRatio(1)
                                    .Shadow(new BoxShadowStyle
                                    {
                                        Color = Colors.Grey.Darken1,
                                        Blur = 5,
                                        Spread = spread
                                    })
                                    .Background(Colors.White);
                            }
                        });
                });
            })
            .GenerateImages(x => $"shadow-spread.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void NoBlur()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(50);
                    page.PageColor(Colors.White);

                    page.Content()
                        .Row(row =>
                        {
                            row.Spacing(50);

                            row.ConstantItem(100)
                                .AspectRatio(1)
                                .Shadow(new BoxShadowStyle
                                {
                                    Color = Colors.Grey.Lighten1,
                                    Blur = 0, 
                                    OffsetX = 8,
                                    OffsetY = 8
                                })
                                .Border(1, Colors.Black)
                                .Background(Colors.White);
                            
                            row.ConstantItem(100)
                                .AspectRatio(1)
                                .Shadow(new BoxShadowStyle
                                {
                                    Color = Colors.Grey.Lighten1,
                                    Blur = 0,
                                    OffsetX = 8,
                                    OffsetY = 8
                                })
                                .Border(1, Colors.Black)
                                .CornerRadius(16)
                                .Background(Colors.White);
                        });
                });
            })
            .GenerateImages(x => $"shadow-no-blur.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
}