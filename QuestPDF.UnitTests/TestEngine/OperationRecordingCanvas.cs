using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using QuestPDF.UnitTests.TestEngine.Operations;
using SkiaSharp;

namespace QuestPDF.UnitTests.TestEngine
{
    internal class OperationRecordingCanvas : ICanvas
    {
        public ICollection<OperationBase> Operations { get; } = new List<OperationBase>();

        public void Translate(Position vector) => Operations.Add(new CanvasTranslateOperation(vector));
        public void Rotate(float angle) => Operations.Add(new CanvasRotateOperation(angle));
        public void Scale(float scaleX, float scaleY) => Operations.Add(new CanvasScaleOperation(scaleX, scaleY));

        public void DrawRectangle(Position vector, Size size, string color) => Operations.Add(new CanvasDrawRectangleOperation(vector, size, color));
        public void DrawText(SKTextBlob skTextBlob, Position position, TextStyle style) => throw new NotImplementedException();
        public void DrawImage(SKImage image, Position position, Size size) => Operations.Add(new CanvasDrawImageOperation(position, size));
        public void DrawPicture(SKPicture picture) => throw new NotImplementedException();

        public void DrawHyperlink(string url, Size size) => throw new NotImplementedException();
        public void DrawSectionLink(string sectionName, Size size) => throw new NotImplementedException();
        public void DrawSection(string sectionName) => throw new NotImplementedException();
    }
}