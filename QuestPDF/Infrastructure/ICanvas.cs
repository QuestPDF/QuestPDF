using QuestPDF.Drawing;
using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    internal interface ICanvas
    {
        void Translate(Position vector);
        
        void DrawRectangle(Position vector, Size size, string color);
        void DrawCorner(Position first, Position center, Position last, Position cornerCenter, string color);
        void DrawText(SKTextBlob skTextBlob, Position position, TextStyle style);
        void DrawImage(SKImage image, Position position, Size size);

        void DrawHyperlink(string url, Size size);
        void DrawSectionLink(string sectionName, Size size);
        void DrawSection(string sectionName);
        
        void Rotate(float angle);
        void Scale(float scaleX, float scaleY);
    }
}