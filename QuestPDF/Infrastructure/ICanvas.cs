using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    internal interface ICanvas
    {
        void Translate(Position vector);
        
        void DrawFilledRectangle(Position vector, Size size, string color);
        void DrawStrokedRectangle(Size size, string color, float width);
        void DrawText(string text, Position position, TextStyle style);
        void DrawImage(SKImage image, Position position, Size size);

        void DrawExternalLink(string url, Size size);
        void DrawLocationLink(string locationName, Size size);
        void DrawLocation(string locationName);
        
        void Rotate(float angle);
        void Scale(float scaleX, float scaleY);
    }
}