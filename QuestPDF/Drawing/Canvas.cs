using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal class Canvas : ICanvas
    {
        private SKCanvas SkiaCanvas { get; }
        
        public Canvas(SKCanvas skiaCanvas)
        {
            SkiaCanvas = skiaCanvas;
        }
        
        ~Canvas()
        {
            SkiaCanvas.Dispose();
        }

        public void Translate(Position vector)
        {
            SkiaCanvas.Translate(vector.X, vector.Y);
        }

        public void DrawRectangle(Position vector, Size size, string color)
        {
            if (size.Width < Size.Epsilon || size.Height < Size.Epsilon)
                return;

            var paint = color.ColorToPaint();
            SkiaCanvas.DrawRect(vector.X, vector.Y, size.Width, size.Height, paint);
        }

        public void DrawText(string text, Position vector, TextStyle style)
        {
            SkiaCanvas.DrawText(text, vector.X, vector.Y, style.ToPaint());
        }

        public void DrawImage(SKImage image, Position vector, Size size)
        {
            SkiaCanvas.DrawImage(image, new SKRect(vector.X, vector.Y, size.Width, size.Height));
        }

        public void DrawLink(string url, Size size)
        {
            SkiaCanvas.DrawUrlAnnotation(new SKRect(0, 0, size.Width, size.Height), url);
        }
    }
}