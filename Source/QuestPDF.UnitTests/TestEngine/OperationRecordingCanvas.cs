using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;
using QuestPDF.UnitTests.TestEngine.Operations;

namespace QuestPDF.UnitTests.TestEngine
{
    internal sealed class OperationRecordingCanvas : IDrawingCanvas
    {
        public ICollection<OperationBase> Operations { get; } = new List<OperationBase>();
        
        public DocumentPageSnapshot GetSnapshot() => throw new NotImplementedException();
        public void DrawSnapshot(DocumentPageSnapshot snapshot) => throw new NotImplementedException();
        
        public void Save() => throw new NotImplementedException();
        public void Restore() => throw new NotImplementedException();
        
        public void SetZIndex(int index) => throw new NotImplementedException();
        public int GetZIndex() => throw new NotImplementedException();
        
        public SkCanvasMatrix GetCurrentMatrix() => throw new NotImplementedException();
        public void SetMatrix(SkCanvasMatrix matrix) => throw new NotImplementedException();

        public void Translate(Position vector) => Operations.Add(new CanvasTranslateOperation(vector));
        public void Scale(float scaleX, float scaleY) => Operations.Add(new CanvasScaleOperation(scaleX, scaleY));
        public void Rotate(float angle) => Operations.Add(new CanvasRotateOperation(angle));

        public void DrawLine(Position start, Position end, SkPaint paint) => throw new NotImplementedException();
        public void DrawRectangle(Position vector, Size size, SkPaint paint) => throw new NotImplementedException();
        public void DrawComplexBorder(SkRoundedRect innerRect, SkRoundedRect outerRect, SkPaint paint) => throw new NotImplementedException();
        public void DrawShadow(SkRoundedRect shadowRect, SkBoxShadow shadow) => throw new NotImplementedException();
        public void DrawParagraph(SkParagraph paragraph, int lineFrom, int lineTo) => throw new NotImplementedException();
        public void DrawImage(SkImage image, Size size) => Operations.Add(new CanvasDrawImageOperation(Position.Zero, size));
        public void DrawPicture(SkPicture picture) => throw new NotImplementedException();
        public void DrawSvgPath(string path, Color color) => throw new NotImplementedException();
        public void DrawSvg(SkSvgImage svgImage, Size size) => throw new NotImplementedException();
        
        public void DrawOverflowArea(SkRect area) => throw new NotImplementedException();
        public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace) => throw new NotImplementedException();
        public void ClipRectangle(SkRect clipArea) => throw new NotImplementedException();
        public void ClipRoundedRectangle(SkRoundedRect clipArea) => throw new NotImplementedException();
        
        public void DrawHyperlink(Size size, string url, string? description) => throw new NotImplementedException();
        public void DrawSectionLink(Size size, string sectionName, string? description) => throw new NotImplementedException();
        public void DrawSection(string sectionName) => throw new NotImplementedException();
        
        public void MarkCurrentContentAsArtifact(bool isArtifact) => throw new NotImplementedException();
        public void SetSemanticNodeId(int nodeId) => throw new NotImplementedException();
    }
}