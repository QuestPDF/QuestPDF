using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.DocumentationExamples;

public class ImageExamples
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
                    page.MaxSize(new PageSize(400, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Grid(grid =>
                        {
                            grid.Columns(2);
                            grid.Spacing(10);
                            
                            grid.Item(2).Text("My photo gallery:").Bold();
                            
                            grid.Item().Image("Resources/Photos/photo-gallery-1.jpg");
                            grid.Item().Image("Resources/Photos/photo-gallery-2.jpg");
                            grid.Item().Image("Resources/Photos/photo-gallery-3.jpg");
                            grid.Item().Image("Resources/Photos/photo-gallery-4.jpg");
                        });
                });
            })
            .GenerateImages(x => "image-example.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void ImageScaling()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(1000, 1500));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Item().PaddingBottom(5).Text("FitWidth").Bold();
                            column.Item()
                                .Width(200)
                                .Height(150)
                                .Border(4)
                                .BorderColor(Colors.Red.Medium)
                                .Image("Resources/Photos/photo.jpg")
                                .FitWidth();

                            column.Item().Height(15);

                            column.Item().PaddingBottom(5).Text("FitHeight").Bold();
                            column.Item()
                                .Width(200)
                                .Height(100)
                                .Border(4)
                                .BorderColor(Colors.Red.Medium)
                                .Image("Resources/Photos/photo.jpg")
                                .FitHeight();
                            
                            column.Item().Height(15);

                            column.Item().PaddingBottom(5).Text("FitArea 1").Bold();
                            column.Item()
                                .Width(200)
                                .Height(100)
                                .Border(4)
                                .BorderColor(Colors.Red.Medium)
                                .Image("Resources/Photos/photo.jpg")
                                .FitArea();
                            
                            column.Item().Height(15);
                            
                            column.Item().PaddingBottom(5).Text("FitArea 2").Bold();
                            column.Item()
                                .Width(200)
                                .Height(150)
                                .Border(4)
                                .BorderColor(Colors.Red.Medium)
                                .Image("Resources/Photos/photo.jpg")
                                .FitArea();
                            
                            column.Item().Height(15);

                            column.Item().PaddingBottom(5).Text("FitUnproportionally").Bold();
                            column.Item()
                                .Width(200)
                                .Height(50)
                                .Border(4)
                                .BorderColor(Colors.Red.Medium)
                                .Image("Resources/Photos/photo.jpg")
                                .FitUnproportionally();
                        });
                });
            })
            .GenerateImages(x => "image-scaling.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void DpiSetting()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(400, 1000));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);
    
                            // lower raster dpi = lower resolution, pixelation
                            column
                                .Item()
                                .Image("Resources/Photos/photo.jpg")
                                .WithRasterDpi(16);
    
                            // higher raster dpi = higher resolution
                            column
                                .Item()
                                .Image("Resources/Photos/photo.jpg")
                                .WithRasterDpi(288);
                        });
                });
            })
            .GenerateImages(x => "image-dpi.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void CompressionSetting()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(400, 1000));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);
    
                            // low quality = smaller output file
                            column
                                .Item()
                                .Image("Resources/Photos/photo.jpg")
                                .WithCompressionQuality(ImageCompressionQuality.VeryLow);
        
                            // high quality / fidelity = larger output file
                            column
                                .Item()
                                .Image("Resources/Photos/photo.jpg")
                                .WithCompressionQuality(ImageCompressionQuality.VeryHigh);
                        });
                });
            })
            .GenerateImages(x => "image-compression.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void GlobalSettings()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Content().Image("Resources/Photos/photo.jpg");
                });
            })
            .WithSettings(new DocumentSettings
            {
                // default: ImageCompressionQuality.High;
                ImageCompressionQuality = ImageCompressionQuality.Medium,

                // default: 288
                ImageRasterDpi = 14
            })
            .GeneratePdf("image-global-settings.pdf");
    }
    
    [Test]
    public void SharedImages()
    {
        var image = Image.FromFile("Resources/checkbox.png");
        
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(350, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(15);
                            
                            foreach (var i in Enumerable.Range(0, 5))
                            {
                                column.Item().Row(row =>
                                {
                                    row.AutoItem().Width(28).Image(image);
                                    row.RelativeItem().PaddingLeft(8).AlignMiddle().Text(Placeholders.Label());
                                });
                            }
                        });
                });
            })
            .GenerateImages(x => "image-shared.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void DynamicImage()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.MinSize(new PageSize(0, 0));
                    page.MaxSize(new PageSize(350, 1000));
                    page.DefaultTextStyle(x => x.FontSize(20));
                    page.PageColor(Colors.Grey.Lighten3);
                    page.Margin(25);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);

                            column.Item().Text(text =>
                            {
                                text.Span("The national flag of Poland").Bold();
                                text.Span(" consists of two horizontal stripes of equal width, the upper one white and the lower one red.");
                            });
                            
                            column.Item()
                                .AspectRatio(80 / 50f)
                                .Border(2)
                                .Image(GenerateNationalFlagOfPoland);
                        });
                    
                    byte[]? GenerateNationalFlagOfPoland(GenerateDynamicImageDelegatePayload context)
                    {
                        using var whitePaint = new SKPaint
                        {
                            Color = SKColors.White,
                        };
                                    
                        using var redPaint = new SKPaint
                        {
                            Color = SKColor.Parse("#BB0A30"),
                        };

                        using var bitmap = new SKBitmap(context.ImageSize.Width, context.ImageSize.Height);
                        using var canvas = new SKCanvas(bitmap);
                                    
                        canvas.DrawRect(0, 0, context.ImageSize.Width, context.ImageSize.Height / 2, whitePaint);
                        canvas.DrawRect(0, context.ImageSize.Height / 2, context.ImageSize.Width, context.ImageSize.Height, redPaint);
                        canvas.Flush();

                        using var content = bitmap.Encode(SKEncodedImageFormat.Png, 100);
                        return content.ToArray();
                    }
                });
            })
            .GenerateImages(x => "image-dynamic.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
    
    [Test]
    public void SvgSupport()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A7.Portrait());
                    page.Margin(25);

                    // page.Content()
                    //     .Svg("pdf-icon.svg")
                    //     .FitArea();

                    var svgContent = File.ReadAllText("Resources/pdf-icon.svg");
                    
                    page.Content()
                        .Svg(svgContent);
                });
            })
            .GeneratePdf("image-svg.pdf");
    }
}