using QuestPDF.Drawing;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Infrastructure
{
    internal interface IDrawingCanvas
    {
        DocumentPageSnapshot GetSnapshot();
        void DrawSnapshot(DocumentPageSnapshot snapshot);
        
        void Save();
        void Restore();

        void SetZIndex(int index);
        int GetZIndex();
        
        SkCanvasMatrix GetCurrentMatrix();
        void SetMatrix(SkCanvasMatrix matrix);
        
        void Translate(Position vector);
        void Scale(float scaleX, float scaleY);
        void Rotate(float angle);
        
        void DrawLine(Position start, Position end, SkPaint paint);
        void DrawRectangle(Position vector, Size size, SkPaint paint);
        void DrawComplexBorder(SkRoundedRect innerRect, SkRoundedRect outerRect, SkPaint paint);
        void DrawShadow(SkRoundedRect shadowRect, SkBoxShadow shadow);
        void DrawParagraph(SkParagraph paragraph, int lineFrom, int lineTo);
        void DrawImage(SkImage image, Size size);
        void DrawPicture(SkPicture picture);
        void DrawSvgPath(string path, Color color);
        void DrawSvg(SkSvgImage svgImage, Size size);

        void DrawOverflowArea(SkRect area);
        void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace);
        void ClipRectangle(SkRect clipArea);
        void ClipRoundedRectangle(SkRoundedRect clipArea);
        
        void DrawHyperlink(Size size, string url, string? description);
        void DrawSectionLink(Size size, string sectionName, string? description);
        void DrawSection(string sectionName);
        
        void MarkCurrentContentAsArtifact(bool isArtifact);
        void SetSemanticNodeId(int nodeId);
    }
}