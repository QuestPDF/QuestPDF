using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing
{
    internal sealed class FreeCanvas : ICanvas, IDocumentCanvas
    {
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
            
        }

        public void Restore()
        {
            
        }

        public void SetZIndex(int index)
        {
            
        }

        public int GetZIndex()
        {
            return 0;
        }
        
        public SkCanvasMatrix GetCurrentMatrix()
        {
            return default;
        }

        public void SetMatrix(SkCanvasMatrix matrix)
        {
            
        }

        public void Translate(Position vector)
        {
            
        }
        
        public void Scale(float scaleX, float scaleY)
        {
            
        }
        
        public void Rotate(float angle)
        {
            
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

        #endregion
    }
}