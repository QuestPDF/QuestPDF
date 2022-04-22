using System.Linq;
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

        public void DrawRectangle(Position vector, Size size, string color)
        {
            if (size.Width < Size.Epsilon || size.Height < Size.Epsilon)
                return;

            var paint = color.ColorToPaint();
            Canvas.DrawRect(vector.X, vector.Y, size.Width, size.Height, paint);
        }

        public void DrawText(string text, Position vector, TextStyle style)
        {
            if (text == string.Empty)
                return;

            var paint = style.ToPaint();
            var runs = FontFallback.BuildRuns(text, paint.Typeface).ToArray();
            var applyTranslation = runs.Length > 1;
            float totalOffset = 0;

            void DrawSegment(string segment, SKPaint paint)
            {
                Canvas.DrawText(segment, vector.X, vector.Y, paint);

                if (!applyTranslation)
                    return;

                var offset = paint.MeasureText(segment);
                Canvas.Translate(offset, 0);
                totalOffset += offset;
            }

            foreach (var run in runs)
            {
                var segment = text.Substring(run.Start, run.End - run.Start);

                //No font fallback needed, draw normally.
                if (paint.Typeface == run.Typeface)
                {
                    DrawSegment(segment, paint);
                    continue;
                }

                //Create a clone of the SKPaint and override the current typeface with the fallback typeface.
                using var fallbackPaint = paint.Clone();
                fallbackPaint.Typeface = run.Typeface;
                DrawSegment(segment, fallbackPaint);
            }

            if (totalOffset > 0)
                Canvas.Translate(-totalOffset, 0);
        }

        public void DrawImage(SKImage image, Position vector, Size size)
        {
            Canvas.DrawImage(image, new SKRect(vector.X, vector.Y, size.Width, size.Height));
        }

        public void DrawHyperlink(string url, Size size)
        {
            Canvas.DrawUrlAnnotation(new SKRect(0, 0, size.Width, size.Height), url);
        }
        
        public void DrawSectionLink(string sectionName, Size size)
        {
            Canvas.DrawLinkDestinationAnnotation(new SKRect(0, 0, size.Width, size.Height), sectionName);
        }

        public void DrawSection(string sectionName)
        {
            Canvas.DrawNamedDestinationAnnotation(new SKPoint(0, 0), sectionName);
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