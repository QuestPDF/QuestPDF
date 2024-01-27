using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing
{
    internal sealed class FreeCanvas : ICanvas, IRenderingCanvas
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
        
        public void Translate(Position vector)
        {
            
        }

        public void DrawFilledRectangle(Position vector, Size size, string color)
        {
            
        }

        public void DrawStrokeRectangle(Position vector, Size size, float strokeWidth, string color)
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

        public void DrawSvgPath(string path, uint color)
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
            
        }

        public void Scale(float scaleX, float scaleY)
        {
            
        }

        #endregion
    }
}