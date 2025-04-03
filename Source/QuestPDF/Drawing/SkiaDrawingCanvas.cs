using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing
{
    internal class SkiaDrawingCanvas : IDrawingCanvas, IDisposable
    {
        public float Width { get; }
        public float Height { get; }
        
        public SkiaDrawingCanvas(float width, float height)
        {
            Width = width;
            Height = height;
        }
        
        ~SkiaDrawingCanvas()
        {
            Dispose();
        }
        
        public void Dispose()
        {
            CurrentCanvas?.Dispose();
            CurrentCanvas = null;

            foreach (var layer in ZIndexCanvases.Values)
            {
                layer.Canvas.Dispose();
                layer.PictureRecorder.Dispose();
            }
            
            ZIndexCanvases.Clear();
            
            GC.SuppressFinalize(this);
        }
        
        #region ZIndex
        
        private SkCanvas CurrentCanvas { get; set; }
        
        private int CurrentZIndex { get; set; } = 0;
        private IDictionary<int, (SkPictureRecorder PictureRecorder, SkCanvas Canvas)> ZIndexCanvases { get; } = new Dictionary<int, (SkPictureRecorder, SkCanvas)>();
        private ICollection<DocumentPageSnapshot> InternalDocumentPageSnapshots { get; } = new List<DocumentPageSnapshot>(0);
        
        SkCanvas GetCanvasForZIndex(int zIndex)
        {
            if (ZIndexCanvases.TryGetValue(zIndex, out var value))
                return value.Canvas;
            
            var pictureRecorder = new SkPictureRecorder();
            var canvas = pictureRecorder.BeginRecording(Width, Height);
            
            ZIndexCanvases.Add(zIndex, (pictureRecorder, canvas));
            return canvas;
        }
        
        #endregion
        
        #region ICanvas

        public DocumentPageSnapshot GetSnapshot()
        { 
            return new DocumentPageSnapshot
            {
                Layers = ZIndexCanvases
                    .Select(zindex =>
                    {
                        using var pictureRecorder = zindex.Value.PictureRecorder;
                        var picture = pictureRecorder.EndRecording();
                        
                        zindex.Value.Canvas.Dispose();
                        
                        return new DocumentPageSnapshot.LayerSnapshot
                        {
                            ZIndex = zindex.Key,
                            Picture = picture
                        };
                    })
                    .ToList()
            };
        }

        public void DrawSnapshot(DocumentPageSnapshot snapshot)
        {
            foreach (var snapshotLayer in snapshot.Layers.OrderBy(x => x.ZIndex))
            {
                ZIndexCanvases[snapshotLayer.ZIndex].Canvas.DrawPicture(snapshotLayer.Picture);
            }
        }

        public void Save()
        {
            CurrentCanvas.Save();
        }

        public void Restore()
        {
            CurrentCanvas.Restore();
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