using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    public delegate void DrawOnCanvas(SKCanvas canvas, Size availableSpace);
    
    internal class Canvas : Element
    {
        public DrawOnCanvas Handler { get; set; }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            return new FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            var skiaCanvas = (Canvas as Drawing.SkiaCanvasBase)?.Canvas;
            
            if (Handler == null || skiaCanvas == null)
                return;

            var currentMatrix = skiaCanvas.TotalMatrix;
            Handler.Invoke(skiaCanvas, availableSpace);
            skiaCanvas.SetMatrix(currentMatrix);
        }
    }
}