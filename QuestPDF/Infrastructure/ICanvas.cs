using System.Collections.Generic;
using SkiaSharp;

namespace QuestPDF.Infrastructure
{
    internal interface ICanvas
    {
        void Translate(Position vector);
        
        void DrawRectangle(Position vector, Size size, string color);
        void DrawText(string text, Position position, TextStyle style);
        void DrawImage(SKImage image, Position position, Size size);

        void DrawExternalLink(string url, Size size);
        void DrawLocationLink(string locationName, Size size);
        void DrawLocation(string locationName);
    }
}