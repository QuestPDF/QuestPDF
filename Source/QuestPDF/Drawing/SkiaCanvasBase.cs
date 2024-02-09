using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing
{
    internal abstract class SkiaCanvasBase : ICanvas, IRenderingCanvas
    {
        internal SkCanvas Canvas { get; set; }

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
            var lineColor = Colors.Red.Medium;
            const byte lineOpacity = 64;
        
            // implementation
            var indicatorColor = lineColor.WithAlpha(lineOpacity);
            var position = new SkRect(0, 0, CurrentPageSize.Width, CurrentPageSize.Height);
            Canvas.DrawFilledRectangle(position, indicatorColor);
        }
        
        #endregion
        
        #region ICanvas
        
        public void Save()
        {
            Canvas.Save();
        }

        public void Restore()
        {
            Canvas.Restore();
        }
        
        public void Translate(Position vector)
        {
            Canvas.Translate(vector.X, vector.Y);
        }

        public void DrawFilledRectangle(Position vector, Size size, Color color)
        {
            if (size.Width < Size.Epsilon || size.Height < Size.Epsilon)
                return;

            var position = new SkRect(vector.X, vector.Y, vector.X + size.Width, vector.Y + size.Height);
            Canvas.DrawFilledRectangle(position, color);
        }
        
        public void DrawStrokeRectangle(Position vector, Size size, float strokeWidth, Color color)
        {
            if (size.Width < Size.Epsilon || size.Height < Size.Epsilon)
                return;

            var position = new SkRect(vector.X, vector.Y, vector.X + size.Width, vector.Y + size.Height);
            Canvas.DrawStrokeRectangle(position, strokeWidth, color);
        }

        public void DrawParagraph(SkParagraph paragraph)
        {
            Canvas.DrawParagraph(paragraph);
        }

        public void DrawImage(SkImage image, Size size)
        {
            Canvas.DrawImage(image, size.Width, size.Height);
        }

        public void DrawPicture(SkPicture picture)
        {
            Canvas.DrawPicture(picture);
        }

        public void DrawSvgPath(string path, Color color)
        {
            Canvas.DrawSvgPath(path, color);
        }

        public void DrawSvg(SkSvgImage svgImage, Size size)
        {
            Canvas.DrawSvg(svgImage, size.Width, size.Height);
        }
        
        public void DrawOverflowArea(SkRect area)
        {
            Canvas.DrawOverflowArea(area);
        }
    
        public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace)
        {
            Canvas.ClipOverflowArea(availableSpace, requiredSpace);
        }
    
        public void ClipRectangle(SkRect clipArea)
        {
            Canvas.ClipRectangle(clipArea);
        }
        
        public void DrawHyperlink(string url, Size size)
        {
            Canvas.AnnotateUrl(size.Width, size.Height, url);
        }
        
        public void DrawSectionLink(string sectionName, Size size)
        {
            Canvas.AnnotateDestinationLink(size.Width, size.Height, sectionName);
        }

        public void DrawSection(string sectionName)
        {
            Canvas.AnnotateDestination(sectionName);
        }

        public void Rotate(float angle)
        {
            Canvas.Rotate(angle);
        }

        public void Scale(float scaleX, float scaleY)
        {
            Canvas.Scale(scaleX, scaleY);
        }
        
        #endregion
    }
}