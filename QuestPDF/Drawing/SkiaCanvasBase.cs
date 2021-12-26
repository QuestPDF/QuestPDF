using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Drawing
{
    internal abstract class SkiaCanvasBase : ICanvas, IRenderingCanvas
    {
        internal SKCanvas Canvas { get; set; }

        public abstract void BeginDocument();
        public abstract void EndDocument();
        
        public abstract void BeginPage(Size size);
        public abstract void EndPage();
        
        public void Translate(Position vector)
        {
            Canvas.Translate(vector.X, vector.Y);
        }

        public void DrawFilledRectangle(Position vector, Size size, string color)
        {
            if (size.Width < Size.Epsilon || size.Height < Size.Epsilon)
                return;

            var paint = SkiaCache.GetRectangleFillPaint(color);
            Canvas.DrawRect(vector.X, vector.Y, size.Width, size.Height, paint);
        }
        
        public void DrawStrokedRectangle(Size size, string color, float width)
        {
            var paint = SkiaCache.GetRectangleStrokePaint(color, width);
            Canvas.DrawRect(0, 0, size.Width, size.Height, paint);
        }

        public void DrawText(string text, Position vector, TextStyle style)
        {
            Canvas.DrawText(text, vector.X, vector.Y, style.ToPaint());
        }

        public void DrawImage(SKImage image, Position vector, Size size)
        {
            Canvas.DrawImage(image, new SKRect(vector.X, vector.Y, size.Width, size.Height));
        }

        public void DrawExternalLink(string url, Size size)
        {
            Canvas.DrawUrlAnnotation(new SKRect(0, 0, size.Width, size.Height), url);
        }
        
        public void DrawLocationLink(string locationName, Size size)
        {
            Canvas.DrawLinkDestinationAnnotation(new SKRect(0, 0, size.Width, size.Height), locationName);
        }

        public void DrawLocation(string locationName)
        {
            Canvas.DrawNamedDestinationAnnotation(new SKPoint(0, 0), locationName);
        }

        public void Rotate(float angle)
        {
            Canvas.RotateDegrees(angle);
        }

        public void Scale(float scaleX, float scaleY)
        {
            Canvas.Scale(scaleX, scaleY);
        }
    }
}