using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing.DrawingCanvases
{
    internal sealed class FreeDrawingCanvas : IDrawingCanvas
    {
        private Stack<SkCanvasMatrix> MatrixStack { get; } = new();
        private SkCanvasMatrix CurrentMatrix { get; set; } = SkCanvasMatrix.Identity;
        private int CurrentZIndex { get; set; } = 0;
        
        public DocumentPageSnapshot GetSnapshot()
        {
            return new DocumentPageSnapshot();
        }

        public void DrawSnapshot(DocumentPageSnapshot snapshot)
        {
            
        }

        public void Save()
        {
            MatrixStack.Push(CurrentMatrix);
        }

        public void Restore()
        {
            CurrentMatrix = MatrixStack.Pop();
        }

        public void SetZIndex(int index)
        {
            CurrentZIndex = index;
        }

        public int GetZIndex()
        {
            return CurrentZIndex;
        }
        
        public SkCanvasMatrix GetCurrentMatrix()
        {
            return CurrentMatrix;
        }

        public void SetMatrix(SkCanvasMatrix matrix)
        {
            CurrentMatrix = matrix;
        }

        public void Translate(Position vector)
        {
            CurrentMatrix *= SkCanvasMatrix.CreateTranslation(vector.X, vector.Y);
        }
        
        public void Scale(float scaleX, float scaleY)
        {
            CurrentMatrix *= SkCanvasMatrix.CreateScale(scaleX, scaleY);
        }
        
        public void Rotate(float angle)
        {
            CurrentMatrix *= SkCanvasMatrix.CreateRotation(angle);
        }

        public void DrawFilledRectangle(Position vector, Size size, Color color)
        {
            
        }

        public void DrawStrokeRectangle(Position vector, Size size, float strokeWidth, Color color)
        {
            
        }
        
        public void DrawParagraph(SkParagraph paragraph, int lineFrom, int lineTo)
        {
            
        }

        public void DrawImage(SkImage image, Size size)
        {
            
        }

        public void DrawPicture(SkPicture picture)
        {
            
        }

        public void DrawSvgPath(string path, Color color)
        {
            
        }

        public void DrawSvg(SkSvgImage svgImage, Size size)
        {
            
        }

        public void DrawOverflowArea(SkRect area)
        {
            
        }

        public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace)
        {
            
        }

        public void ClipRectangle(SkRect clipArea)
        {
            
        }
        
        public void DrawHyperlink(string url, Size size)
        {
           
        }

        public void DrawSectionLink(string sectionName, Size size)
        {
            
        }

        public void DrawSection(string sectionName)
        {
            
        }
    }
}