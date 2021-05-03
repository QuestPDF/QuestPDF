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

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var skiaCanvas = (canvas as Drawing.Canvas)?.SkiaCanvas;
            
            if (Handler == null || skiaCanvas == null)
                return;

            var currentMatrix = skiaCanvas.TotalMatrix;
            Handler.Invoke(skiaCanvas, availableSpace);
            skiaCanvas.SetMatrix(currentMatrix);
        }
    }
}