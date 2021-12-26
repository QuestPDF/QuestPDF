using System;
using System.Collections.Generic;
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

        public void DrawFilledRectangle(Position vector, Size size, string color) => Operations.Add(new CanvasDrawFilledRectangleOperation(vector, size, color));
        public void DrawStrokedRectangle(Size size, string color, float width) => Operations.Add(new CanvasDrawStrokedRectangleOperation(size, color, width));
        public void DrawText(string text, Position position, TextStyle style) => Operations.Add(new CanvasDrawTextOperation(text, position, style));
        public void DrawImage(SKImage image, Position position, Size size) => Operations.Add(new CanvasDrawImageOperation(position, size));
        
        public void DrawExternalLink(string url, Size size) => throw new NotImplementedException();
        public void DrawLocationLink(string locationName, Size size) => throw new NotImplementedException();
        public void DrawLocation(string locationName) => throw new NotImplementedException();
    }
}