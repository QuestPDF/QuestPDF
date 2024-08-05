using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing
{
    internal sealed class FreeCanvas : ICanvas, IRenderingCanvas
    {
        #region Internal State

        private Stack<CanvasMatrix> MatrixStack { get; set; } = new();
        private CanvasMatrix Matrix { get; set; } = new();

        #endregion
        
        #region IRenderingCanvas

        public bool DocumentContentHasLayoutOverflowIssues { get; set; }
        
        public void BeginDocument()
        {
            
        }

        public void EndDocument()
        {
            
        }

        public void BeginPage(Size size)
        {
            
        }

        public void EndPage()
        {
            
        }

        public void MarkCurrentPageAsHavingLayoutIssues()
        {
            
        }

        #endregion

        #region ICanvas

        public void Save()
        {
            MatrixStack.Push(Matrix);
        }

        public void Restore()
        {
            Matrix = MatrixStack.Pop();
        }
        
        public void Translate(Position vector)
        {
            Matrix = Matrix.Translate(vector.X, vector.Y);
        }

        public void DrawFilledRectangle(Position vector, Size size, Color color)
        {
            
        }

        public void DrawStrokeRectangle(Position vector, Size size, float strokeWidth, Color color)
        {
            
        }
        
        public void DrawParagraph(SkParagraph paragraph)
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

        public void Rotate(float angle)
        {
            Matrix = Matrix.Rotate(angle);
        }

        public void Scale(float scaleX, float scaleY)
        {
            Matrix = Matrix.Scale(scaleX, scaleY);
        }
        
        public CanvasMatrix GetCurrentMatrix()
        {
            return Matrix;
        }

        #endregion
    }
}