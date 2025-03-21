using System;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;
using SkiaSharp;

namespace QuestPDF.UnitTests.TestEngine
{
    internal sealed class MockCanvas : ICanvas
    {
        public Action<Position> TranslateFunc { get; set; }
        public Action<float> RotateFunc { get; set; }
        public Action<float, float> ScaleFunc { get; set; }
        public Action<SkImage, Position, Size> DrawImageFunc { get; set; }
        public Action<Position, Size, Color> DrawRectFunc { get; set; }

        public void Save() => throw new NotImplementedException();
        public void Restore() => throw new NotImplementedException();
        
        public void Translate(Position vector) => TranslateFunc(vector);
        public void Rotate(float angle) => RotateFunc(angle);
        public void Scale(float scaleX, float scaleY) => ScaleFunc(scaleX, scaleY);

        public void DrawFilledRectangle(Position vector, Size size, Color color) => DrawRectFunc(vector, size, color);
        public void DrawStrokeRectangle(Position vector, Size size, float strokeWidth, Color color) => throw new NotImplementedException();
        public void DrawParagraph(SkParagraph paragraph, int lineFrom, int lineTo) => throw new NotImplementedException();
        public void DrawImage(SkImage image, Size size) => DrawImageFunc(image, Position.Zero, size);
        public void DrawPicture(SkPicture picture) => throw new NotImplementedException();
        public void DrawSvgPath(string path, Color color) => throw new NotImplementedException();
        public void DrawSvg(SkSvgImage svgImage, Size size) => throw new NotImplementedException();
        
        public void DrawOverflowArea(SkRect area) => throw new NotImplementedException();
        public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace) => throw new NotImplementedException();
        public void ClipRectangle(SkRect clipArea) => throw new NotImplementedException();
        
        public void DrawHyperlink(string url, Size size) => throw new NotImplementedException();
        public void DrawSectionLink(string sectionName, Size size) => throw new NotImplementedException();
        public void DrawSection(string sectionName) => throw new NotImplementedException();
    }
}