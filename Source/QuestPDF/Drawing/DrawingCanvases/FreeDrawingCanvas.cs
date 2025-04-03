using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing.DrawingCanvases
{
    internal sealed class FreeDrawingCanvas : IDrawingCanvas
    {
        public DocumentPageSnapshot GetSnapshot()
        {
            return new DocumentPageSnapshot();
        }

        public void DrawSnapshot(DocumentPageSnapshot snapshot)
        {
            
        }

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
    }
}