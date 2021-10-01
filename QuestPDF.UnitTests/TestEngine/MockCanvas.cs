using System;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.UnitTests.TestEngine
{
    internal class MockCanvas : ICanvas
    {
        public Action<Position> TranslateFunc { get; set; }
        public Action<float> RotateFunc { get; set; }
        public Action<float, float> ScaleFunc { get; set; }
        public Action<SKImage, Position, Size> DrawImageFunc { get; set; }
        public Action<string, Position, TextStyle> DrawTextFunc { get; set; }
        public Action<Position, Size, string> DrawRectFunc { get; set; }

        public void Translate(Position vector) => TranslateFunc(vector);
        public void Rotate(float angle) => RotateFunc(angle);
        public void Scale(float scaleX, float scaleY) => ScaleFunc(scaleX, scaleY);

        public void DrawRectangle(Position vector, Size size, string color) => DrawRectFunc(vector, size, color);
        public void DrawText(string text, Position position, TextStyle style) => DrawTextFunc(text, position, style);
        public void DrawImage(SKImage image, Position position, Size size) => DrawImageFunc(image, position, size);
        
        public void DrawExternalLink(string url, Size size) => throw new NotImplementedException();
        public void DrawLocationLink(string locationName, Size size) => throw new NotImplementedException();
        public void DrawLocation(string locationName) => throw new NotImplementedException();
    }
}