using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.SkiaSharpIntegration;

public static class SkiaSharpHelpers
{
    public static void SkiaSharpSvgCanvas(this IContainer container, Action<SKCanvas, Size> drawOnCanvas)
    {
        container.Svg(size =>
        {
            using var stream = new MemoryStream();

            using (var canvas = SKSvgCanvas.Create(new SKRect(0, 0, size.Width, size.Height), stream))
                drawOnCanvas(canvas, size);
            
            var svgData = stream.ToArray();
            return Encoding.UTF8.GetString(svgData);
        });
    }
    
    public static void SkiaSharpRasterizedCanvas(this IContainer container, Action<SKCanvas, ImageSize> drawOnCanvas)
    {
        container.Image(payload =>
        {
            using var bitmap = new SKBitmap(payload.ImageSize.Width, payload.ImageSize.Height);

            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Scale(payload.ImageSize.Width / payload.AvailableSpace.Width, payload.ImageSize.Height / payload.AvailableSpace.Height);
                drawOnCanvas(canvas, new ImageSize((int)payload.AvailableSpace.Width, (int)payload.AvailableSpace.Height));
            }
        
            return bitmap.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        });
    }
}