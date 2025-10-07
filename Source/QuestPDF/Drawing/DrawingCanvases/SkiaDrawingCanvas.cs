using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing.DrawingCanvases
{
    internal sealed class SkiaDrawingCanvas : IDrawingCanvas, IDisposable
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

        private SkCanvas GetCanvasForZIndex(int zIndex)
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
                var canvas = GetCanvasForZIndex(snapshotLayer.ZIndex);

                canvas.Save();
                canvas.SetCurrentMatrix(SkCanvasMatrix.Identity);
                canvas.DrawPicture(snapshotLayer.Picture);
                canvas.Restore();
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
            var currentMatrix = CurrentCanvas?.GetCurrentMatrix() ?? SkCanvasMatrix.Identity;
            
            CurrentZIndex = index;
            CurrentCanvas = GetCanvasForZIndex(CurrentZIndex);
            
            CurrentCanvas.SetCurrentMatrix(currentMatrix);
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
        
        public void DrawLine(Position start, Position end, SkPaint paint)
        {
            var startPoint = new SkPoint(start.X, start.Y);
            var endPoint = new SkPoint(end.X, end.Y);
            
            CurrentCanvas.DrawLine(startPoint, endPoint, paint);
        }

        public void DrawRectangle(Position vector, Size size, SkPaint paint)
        {
            var position = new SkRect(vector.X, vector.Y, vector.X + size.Width, vector.Y + size.Height);
            CurrentCanvas.DrawRectangle(position, paint);
        }
        
        public void DrawComplexBorder(SkRoundedRect innerRect, SkRoundedRect outerRect, SkPaint paint)
        {
            CurrentCanvas.DrawComplexBorder(innerRect, outerRect, paint);
        }
        
        public void DrawShadow(SkRoundedRect shadowRect, SkBoxShadow shadow)
        {
            CurrentCanvas.DrawShadow(shadowRect, shadow);
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
        
        public void ClipRoundedRectangle(SkRoundedRect clipArea)
        {
            CurrentCanvas.ClipRoundedRectangle(clipArea);
        }
        
        public void DrawHyperlink(Size size, string url, string? description)
        {
            CurrentCanvas.AnnotateUrl(size.Width, size.Height, url, description);
        }
        
        public void DrawSectionLink(Size size, string sectionName, string? description)
        {
            CurrentCanvas.AnnotateDestinationLink(size.Width, size.Height, sectionName, description);
        }

        public void DrawSection(string sectionName)
        {
            CurrentCanvas.AnnotateDestination(sectionName);
        }

        private bool IsCurrentContentArtifact { get; set; } = false;

        public void MarkCurrentContentAsArtifact(bool isArtifact)
        {
            IsCurrentContentArtifact = isArtifact;
        }
        
        public void SetSemanticNodeId(int nodeId)
        {
            var isContentNodeId = nodeId > 0; // artifact is less than 0, nothing is 0
            
            if (IsCurrentContentArtifact && isContentNodeId)
                return;
            
            CurrentCanvas.SetSemanticNodeId(nodeId);
        }
        
        #endregion
    }
}