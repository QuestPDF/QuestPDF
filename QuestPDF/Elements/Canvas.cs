using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    public delegate void DrawOnCanvas(SKCanvas canvas, Size availableSpace);
    
    internal class Canvas : Element, IVisual, ICacheable
    {
        public bool IsRendered { get; set; }
        public bool RepeatContent { get; set; }
        
        public DrawOnCanvas Handler { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (availableSpace.IsNegative())
                return SpacePlan.Wrap();
            
            if (IsRendered && !RepeatContent)
                return SpacePlan.FullRender(Size.Zero);
            
            return SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            IsRendered = true;
            
            var skiaCanvas = (Canvas as Drawing.SkiaCanvasBase)?.Canvas;
            
            if (Handler == null || skiaCanvas == null)
                return;

            var originalMatrix = skiaCanvas.TotalMatrix;
            Handler.Invoke(skiaCanvas, availableSpace);
            skiaCanvas.SetMatrix(originalMatrix);
        }
    }
}