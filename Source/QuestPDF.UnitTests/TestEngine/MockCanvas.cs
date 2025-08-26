﻿using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;
using SkiaSharp;

namespace QuestPDF.UnitTests.TestEngine
{
    internal sealed class MockCanvas : IDrawingCanvas
    {
        public Action<Position> TranslateFunc { get; set; }
        public Action<float> RotateFunc { get; set; }
        public Action<float, float> ScaleFunc { get; set; }
        public Action<SkImage, Position, Size> DrawImageFunc { get; set; }
        public Action<Position, Size, Color> DrawRectFunc { get; set; }

        public DocumentPageSnapshot GetSnapshot() => throw new NotImplementedException();
        public void DrawSnapshot(DocumentPageSnapshot snapshot) => throw new NotImplementedException();

        public void Save() => throw new NotImplementedException();
        public void Restore() => throw new NotImplementedException();
        
        public void SetZIndex(int index) => throw new NotImplementedException();
        public int GetZIndex() => throw new NotImplementedException();
        
        public SkCanvasMatrix GetCurrentMatrix() => throw new NotImplementedException();
        public void SetMatrix(SkCanvasMatrix matrix) => throw new NotImplementedException();

        public void Translate(Position vector) => TranslateFunc(vector);
        public void Scale(float scaleX, float scaleY) => ScaleFunc(scaleX, scaleY);
        public void Rotate(float angle) => RotateFunc(angle);

        public void DrawLine(Position start, Position end, SkPaint paint) => throw new NotImplementedException();
        public void DrawRectangle(Position vector, Size size, SkPaint paint) => throw new NotImplementedException();
        public void DrawComplexBorder(SkRoundedRect innerRect, SkRoundedRect outerRect, SkPaint paint) => throw new NotImplementedException();
        public void DrawShadow(SkRoundedRect shadowRect, SkBoxShadow shadow) => throw new NotImplementedException();
        public void DrawParagraph(SkParagraph paragraph, int lineFrom, int lineTo) => throw new NotImplementedException();
        public void DrawImage(SkImage image, Size size) => DrawImageFunc(image, Position.Zero, size);
        public void DrawPicture(SkPicture picture) => throw new NotImplementedException();
        public void DrawSvgPath(string path, Color color) => throw new NotImplementedException();
        public void DrawSvg(SkSvgImage svgImage, Size size) => throw new NotImplementedException();
        
        public void DrawOverflowArea(SkRect area) => throw new NotImplementedException();
        public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace) => throw new NotImplementedException();
        public void ClipRectangle(SkRect clipArea) => throw new NotImplementedException();
        public void ClipRoundedRectangle(SkRoundedRect clipArea) => throw new NotImplementedException();
        
        public void DrawHyperlink(string url, Size size) => throw new NotImplementedException();
        public void DrawSectionLink(string sectionName, Size size) => throw new NotImplementedException();
        public void DrawSection(string sectionName) => throw new NotImplementedException();
        
        public void SetSemanticNodeId(int nodeId) => throw new NotImplementedException();
    }
}