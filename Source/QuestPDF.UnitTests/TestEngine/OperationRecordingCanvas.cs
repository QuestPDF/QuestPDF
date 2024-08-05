using System;
using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;
using QuestPDF.UnitTests.TestEngine.Operations;

namespace QuestPDF.UnitTests.TestEngine
{
    internal sealed class OperationRecordingCanvas : ICanvas
    {
        public ICollection<OperationBase> Operations { get; } = new List<OperationBase>();
        
        public void Save() => throw new NotImplementedException();
        public void Restore() => throw new NotImplementedException();
        public CanvasMatrix GetCurrentMatrix() => throw new NotImplementedException();
        
        public void Translate(Position vector) => Operations.Add(new CanvasTranslateOperation(vector));
        public void Rotate(float angle) => Operations.Add(new CanvasRotateOperation(angle));
        public void Scale(float scaleX, float scaleY) => Operations.Add(new CanvasScaleOperation(scaleX, scaleY));

        public void DrawFilledRectangle(Position vector, Size size, Color color) => Operations.Add(new CanvasDrawRectangleOperation(vector, size, color));
        public void DrawStrokeRectangle(Position vector, Size size, float strokeWidth, Color color) => throw new NotImplementedException();
        public void DrawParagraph(SkParagraph paragraph) => throw new NotImplementedException();
        public void DrawImage(SkImage image, Size size) => Operations.Add(new CanvasDrawImageOperation(Position.Zero, size));
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