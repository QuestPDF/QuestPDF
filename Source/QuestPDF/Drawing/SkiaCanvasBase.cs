using System.Collections.Generic;
using System.Linq;
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

            SetZIndex(0);
        }

        public virtual void EndPage()
        {
            DrawZIndexContent(Canvas);
            DisposeZIndexCanvases();
            
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
        
        #region ZIndex
        
        private SkCanvas CurrentCanvas { get; set; }
        
        private int CurrentZIndex { get; set; } = 0;
        private IDictionary<int, (SkPictureRecorder PictureRecorder, SkCanvas Canvas)> ZIndexCanvases { get; } = new Dictionary<int, (SkPictureRecorder, SkCanvas)>();
        
        SkCanvas GetCanvasForZIndex(int zIndex)
        {
            if (ZIndexCanvases.TryGetValue(zIndex, out var value))
                return value.Canvas;
            
            var pictureRecorder = new SkPictureRecorder();
            var canvas = pictureRecorder.BeginRecording(CurrentPageSize.Width, CurrentPageSize.Height);
            
            ZIndexCanvases.Add(zIndex, (pictureRecorder, canvas));
            return canvas;
        }

        private void DrawZIndexContent(SkCanvas canvas)
        {
            foreach (var zIndex in ZIndexCanvases.OrderBy(x => x.Key).Select(x => x.Value))
            {
                using var pictureRecorder = zIndex.PictureRecorder;
                using var picture = pictureRecorder.EndRecording();
                zIndex.Canvas.Dispose();
                canvas.DrawPicture(picture);
            }
        }
        
        private void DisposeZIndexCanvases()
        {
            CurrentCanvas.Dispose();
            CurrentCanvas = null;

            foreach (var layer in ZIndexCanvases.Values)
            {
                layer.Canvas.Dispose();
                layer.PictureRecorder.Dispose();
            }
            
            ZIndexCanvases.Clear();
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
        
        public void SetZIndex(int index)
        {
            CurrentZIndex = index;
            CurrentCanvas = GetCanvasForZIndex(CurrentZIndex);
        }

        public int GetZIndex()
        {
            return CurrentZIndex;
        }

        public SkCanvasMatrix GetCurrentMatrix()
        {
            return CurrentCanvas.GetCurrentMatrix();
        }

        public void SetMatrix(SkCanvasMatrix matrix)
        {
            CurrentCanvas.SetCurrentMatrix(matrix);
        }
        
        public void Translate(Position vector)
        {
            CurrentCanvas.Translate(vector.X, vector.Y);
        }
        
        public void Scale(float scaleX, float scaleY)
        {
            CurrentCanvas.Scale(scaleX, scaleY);
        }
        
        public void Rotate(float angle)
        {
            CurrentCanvas.Rotate(angle);
        }

        public void DrawFilledRectangle(Position vector, Size size, Color color)
        {
            if (size.Width < Size.Epsilon || size.Height < Size.Epsilon)
                return;

            var position = new SkRect(vector.X, vector.Y, vector.X + size.Width, vector.Y + size.Height);
            CurrentCanvas.DrawFilledRectangle(position, color);
        }
        
        public void DrawStrokeRectangle(Position vector, Size size, float strokeWidth, Color color)
        {
            if (size.Width < Size.Epsilon || size.Height < Size.Epsilon)
                return;

            var position = new SkRect(vector.X, vector.Y, vector.X + size.Width, vector.Y + size.Height);
            CurrentCanvas.DrawStrokeRectangle(position, strokeWidth, color);
        }

        public void DrawParagraph(SkParagraph paragraph, int lineFrom, int lineTo)
        {
            CurrentCanvas.DrawParagraph(paragraph, lineFrom, lineTo);
        }

        public void DrawImage(SkImage image, Size size)
        {
            CurrentCanvas.DrawImage(image, size.Width, size.Height);
        }

        public void DrawPicture(SkPicture picture)
        {
            CurrentCanvas.DrawPicture(picture);
        }

        public void DrawSvgPath(string path, Color color)
        {
            CurrentCanvas.DrawSvgPath(path, color);
        }

        public void DrawSvg(SkSvgImage svgImage, Size size)
        {
            CurrentCanvas.DrawSvg(svgImage, size.Width, size.Height);
        }
        
        public void DrawOverflowArea(SkRect area)
        {
            CurrentCanvas.DrawOverflowArea(area);
        }
    
        public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace)
        {
            CurrentCanvas.ClipOverflowArea(availableSpace, requiredSpace);
        }
    
        public void ClipRectangle(SkRect clipArea)
        {
            CurrentCanvas.ClipRectangle(clipArea);
        }
        
        public void DrawHyperlink(string url, Size size)
        {
            CurrentCanvas.AnnotateUrl(size.Width, size.Height, url);
        }
        
        public void DrawSectionLink(string sectionName, Size size)
        {
            CurrentCanvas.AnnotateDestinationLink(size.Width, size.Height, sectionName);
        }

        public void DrawSection(string sectionName)
        {
            CurrentCanvas.AnnotateDestination(sectionName);
        }
        
        #endregion
    }
}