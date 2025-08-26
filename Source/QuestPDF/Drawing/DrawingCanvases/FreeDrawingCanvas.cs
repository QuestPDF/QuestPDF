using System;
using System.Collections.Generic;
using System.Numerics;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing.DrawingCanvases
{
    internal sealed class FreeDrawingCanvas : IDrawingCanvas
    {
        private Stack<Matrix4x4> MatrixStack { get; } = new();
        private Matrix4x4 CurrentMatrix { get; set; } = Matrix4x4.Identity;
        private int CurrentZIndex { get; set; } = 0;
        
        public DocumentPageSnapshot GetSnapshot()
        {
            return new DocumentPageSnapshot();
        }

        public void DrawSnapshot(DocumentPageSnapshot snapshot)
        {
            
        }

        public void Save()
        {
            MatrixStack.Push(CurrentMatrix);
        }

        public void Restore()
        {
            CurrentMatrix = MatrixStack.Pop();
        }

        public void SetZIndex(int index)
        {
            CurrentZIndex = index;
        }

        public int GetZIndex()
        {
            return CurrentZIndex;
        }
        
        public SkCanvasMatrix GetCurrentMatrix()
        {
            return SkCanvasMatrix.FromMatrix4x4(CurrentMatrix);
        }

        public void SetMatrix(SkCanvasMatrix matrix)
        {
            CurrentMatrix = matrix.ToMatrix4x4();
        }

        public void Translate(Position vector)
        {
            CurrentMatrix = Matrix4x4.CreateTranslation(vector.X, vector.Y, 0) * CurrentMatrix;
        }
        
        public void Scale(float scaleX, float scaleY)
        {
            CurrentMatrix = Matrix4x4.CreateScale(scaleX, scaleY, 1) * CurrentMatrix;
        }
        
        public void Rotate(float angle)
        {
            CurrentMatrix = Matrix4x4.CreateRotationZ((float)Math.PI * angle / 180f) * CurrentMatrix;
        }

        public void DrawLine(Position start, Position end, SkPaint paint)
        {
            
        }

        public void DrawRectangle(Position vector, Size size, SkPaint paint)
        {
            
        }

        public void DrawComplexBorder(SkRoundedRect innerRect, SkRoundedRect outerRect, SkPaint paint)
        {
            
        }
        
        public void DrawShadow(SkRoundedRect shadowRect, SkBoxShadow shadow)
        {
            
        }
        
        public void DrawParagraph(SkParagraph paragraph, int lineFrom, int lineTo)
        {
            
        }

        public void DrawImage(SkImage image, Size size)
        {
            
        }

        public void DrawPicture(SkPicture picture)
        {
            
        }

        public void DrawSvgPath(string path, Color color)
        {
            
        }

        public void DrawSvg(SkSvgImage svgImage, Size size)
        {
            
        }

        public void DrawOverflowArea(SkRect area)
        {
            
        }

        public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace)
        {
            
        }

        public void ClipRectangle(SkRect clipArea)
        {
            
        }
        
        public void ClipRoundedRectangle(SkRoundedRect clipArea)
        {
            
        }
        
        public void DrawHyperlink(string url, Size size)
        {
           
        }

        public void DrawSectionLink(string sectionName, Size size)
        {
            
        }

        public void DrawSection(string sectionName)
        {
            
        }
        
        public void SetSemanticNodeId(int nodeId)
        {
            
        }
    }
}