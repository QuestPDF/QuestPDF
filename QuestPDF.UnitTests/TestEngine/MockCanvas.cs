using System;
using QuestPDF.Drawing;
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
        public Action<Position, Size, string> DrawRectFunc { get; set; }

        public void Translate(Position vector) => TranslateFunc(vector);
        public void Rotate(float angle) => RotateFunc(angle);
        public void Scale(float scaleX, float scaleY) => ScaleFunc(scaleX, scaleY);

        public void DrawRectangle(Position vector, Size size, string color) => DrawRectFunc(vector, size, color);
        public void DrawText(SKTextBlob skTextBlob, Position position, TextStyle style) => throw new NotImplementedException();
        public void DrawImage(SKImage image, Position position, Size size) => DrawImageFunc(image, position, size);

        public void DrawHyperlink(string url, Size size) => throw new NotImplementedException();
        public void DrawSectionLink(string sectionName, Size size) => throw new NotImplementedException();
        public void DrawSection(string sectionName) => throw new NotImplementedException();
    }
}