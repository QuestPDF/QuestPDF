using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing
{
    internal abstract class SkiaCanvasBase : ICanvas, IRenderingCanvas
    {
        internal SKCanvas Canvas { get; set; }

        #region IRenderingCanvas
        
        public bool DocumentContentHasLayoutOverflowIssues { get; set; }
        
        private Size CurrentPageSize { get; set; } = Size.Zero;
        private bool CurrentPageHasLayoutIssues { get; set; }
        
        public abstract void BeginDocument();
        public abstract void EndDocument();

        public virtual void BeginPage(Size size)
        {
            CurrentPageSize = size;
            CurrentPageHasLayoutIssues = false;
        }

        public virtual void EndPage()
        {
            if (CurrentPageHasLayoutIssues)
                DrawLayoutIssuesIndicatorOnCurrentPage();
        }

        public void MarkCurrentPageAsHavingLayoutIssues()
        {
            CurrentPageHasLayoutIssues = true;
            DocumentContentHasLayoutOverflowIssues = true;
        }
        
        private void DrawLayoutIssuesIndicatorOnCurrentPage()
        {
            // visual configuration
            const string lineColor = Colors.Red.Medium;
            const byte lineOpacity = 64;
            const float borderThickness = 24f;
        
            // implementation
            using var indicatorPaint = new SKPaint
            {
                StrokeWidth = borderThickness * 2, // half of the width will be outside of the page area
                Color = SKColor.Parse(lineColor).WithAlpha(lineOpacity),
                IsStroke = true
            };
        
            Canvas.DrawRect(0, 0, CurrentPageSize.Width, CurrentPageSize.Height, indicatorPaint);
        }
        
        #endregion
        
        #region ICanvas
        
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

        public void DrawText(SKTextBlob skTextBlob, Position position, TextStyle style)
        {
            Canvas.DrawText(skTextBlob, position.X, position.Y, style.ToPaint());
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
        
        #endregion
    }
}