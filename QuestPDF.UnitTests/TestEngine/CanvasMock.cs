using System;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.UnitTests.TestEngine
{
    internal class CanvasMock : ICanvas
    {
        public Action<Position> TranslateFunc { get; set; }
        public Action<SKImage, Position, Size> DrawImageFunc { get; set; }
        public Action<string, Position, TextStyle> DrawTextFunc { get; set; }
        public Action<Position, Size, string> DrawRectFunc { get; set; }

        public void Translate(Position vector) => TranslateFunc(vector);
        public void DrawRectangle(Position vector, Size size, string color) => DrawRectFunc(vector, size, color);
        public void DrawText(string text, Position position, TextStyle style) => DrawTextFunc(text, position, style);
        public void DrawImage(SKImage image, Position position, Size size) => DrawImageFunc(image, position, size);
        
        public void DrawExternalLink(string url, Size size)
        {
            throw new NotImplementedException();
        }

        public void DrawLocationLink(string locationName, Size size)
        {
            throw new NotImplementedException();
        }

        public void DrawLocation(string locationName)
        {
            throw new NotImplementedException();
        }

        public void DrawLink(string url, Size size)
        {
            throw new NotImplementedException();
        }

        public Size MeasureText(string text, TextStyle style)
        {
            return new Size(text.Length * style.Size, style.Size);
        }
    }
}