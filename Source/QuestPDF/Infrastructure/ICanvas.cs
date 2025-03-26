using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Infrastructure
{
    internal interface ICanvas
    {
        void Save();
        void Restore();

        void SetZIndex(int index);
        int GetZIndex();
        
        SkCanvasMatrix GetCurrentMatrix();
        void SetMatrix(SkCanvasMatrix matrix);
        
        void Translate(Position vector);
        void Scale(float scaleX, float scaleY);
        void Rotate(float angle);
        
        void DrawFilledRectangle(Position vector, Size size, Color color);
        void DrawStrokeRectangle(Position vector, Size size, float strokeWidth, Color color);
        void DrawParagraph(SkParagraph paragraph, int lineFrom, int lineTo);
        void DrawImage(SkImage image, Size size);
        void DrawPicture(SkPicture picture);
        void DrawSvgPath(string path, Color color);
        void DrawSvg(SkSvgImage svgImage, Size size);

        void DrawOverflowArea(SkRect area);
        void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace);
        void ClipRectangle(SkRect clipArea);
        
        void DrawHyperlink(string url, Size size);
        void DrawSectionLink(string sectionName, Size size);
        void DrawSection(string sectionName);
    }
}