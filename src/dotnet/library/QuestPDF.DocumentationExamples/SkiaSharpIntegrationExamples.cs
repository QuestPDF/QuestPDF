using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.SkiaSharpIntegration;
using SkiaSharp;

namespace QuestPDF.DocumentationExamples;

public class SkiaSharpIntegrationExamples
{
    [Test]
    public void Svg()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(350, 350);
                    page.Margin(25);

                    page.Content()
                        .Width(300)
                        .Height(300)
                        .SkiaSharpSvgCanvas((canvas, size) =>
                        {
                            var centerX = size.Width / 2;
                            var centerY = size.Height / 2;
                            var radius = Math.Min(centerX, centerY);

                            // draw clock face
                            using var facePaint = new SKPaint
                            {
                                Color = new SKColor(Colors.Blue.Lighten4)
                            };

                            canvas.DrawCircle(centerX, centerY, radius, facePaint);

                            // draw clock ticks
                            using var tickPaint = new SKPaint
                            {
                                Color = new SKColor(Colors.Blue.Darken4), 
                                StrokeWidth = 4, 
                                StrokeCap = SKStrokeCap.Round
                            };

                            canvas.Save();
                            canvas.Translate(centerX, centerY);

                            foreach (var i in Enumerable.Range(0, 12))
                            {
                                canvas.DrawLine(new SKPoint(0, radius * 0.85f), new SKPoint(0, radius * 0.95f), tickPaint);
                                canvas.RotateDegrees(30);
                            }

                            canvas.Restore();

                            // draw clock hands
                            using var hourHandPaint = new SKPaint
                            {
                                Color = new SKColor(Colors.Blue.Darken4),
                                StrokeWidth = 8,
                                StrokeCap = SKStrokeCap.Round
                            };

                            using var minuteHandPaint = new SKPaint
                            {
                                Color = new SKColor(Colors.Blue.Darken2),
                                StrokeWidth = 4,
                                StrokeCap = SKStrokeCap.Round
                            };

                            canvas.Translate(centerX, centerY);

                            canvas.Save();
                            canvas.RotateDegrees(6 * DateTime.Now.Minute);
                            canvas.DrawLine(new SKPoint(0, 0), new SKPoint(0, -radius * 0.7f), minuteHandPaint);
                            canvas.Restore();
                            
                            canvas.Save();
                            canvas.RotateDegrees(30 * DateTime.Now.Hour + DateTime.Now.Minute / 2);
                            canvas.DrawLine(new SKPoint(0, 0), new SKPoint(0, -radius * 0.5f), hourHandPaint);
                            canvas.Restore();
                        });
                });
            })
            .GenerateImages(x => "skiasharp-integration-svg.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.Best, RasterDpi = 144 });
    }
    
    [Test]
    public void Resterized()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(new PageSize(500, 400));
                    page.DefaultTextStyle(x => x.FontSize(20));

                    page.Content()
                        .Padding(25)
                        .SkiaSharpRasterizedCanvas((canvas, size) =>
                        {
                            // add padding to properly display the shadow effect
                            const float padding = 25;
                            canvas.Translate(padding, padding);
                            
                            // load image and scale canvas space
                            using var bitmap = SKBitmap.Decode("Resources/landscape.jpg");
                            
                            var targetBitmapSize = new SKSize(size.Width - 2 * padding, size.Height - 2 * padding);
                            var scale = Math.Min(targetBitmapSize.Width / bitmap.Width, targetBitmapSize.Height / bitmap.Height);
                            canvas.Scale(scale);

                            var drawingArea = new SKRoundRect(new SKRect(0, 0, bitmap.Width, bitmap.Height), 32, 32);
                            
                            // draw drop shadow
                            using var dropShadowFilter = SKImageFilter.CreateDropShadow(8, 8, 16, 16, SKColors.Black);
                            using var paint = new SKPaint
                            {
                                ImageFilter = dropShadowFilter
                            };

                            canvas.DrawRoundRect(drawingArea, paint);
                            
                            // draw image
                            canvas.ClipRoundRect(drawingArea, antialias: true);
                            canvas.DrawBitmap(bitmap, SKPoint.Empty);
                        });
                });
            })
            .GenerateImages(x => "skiasharp-integration-rasterized.webp", new ImageGenerationSettings() { ImageFormat = ImageFormat.Webp, ImageCompressionQuality = ImageCompressionQuality.VeryHigh, RasterDpi = 144 });
    }
}