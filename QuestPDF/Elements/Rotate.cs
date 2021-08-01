using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Rotate : ContainerElement
    {
        public float Angle { get; set; } = 0;
        
        internal override void Draw(Size availableSpace)
        {
            var skiaCanvas = (Canvas as Drawing.SkiaCanvasBase)?.Canvas;
            
            if (skiaCanvas == null)
                return;
            
            var currentMatrix = skiaCanvas.TotalMatrix;
            
            skiaCanvas.RotateDegrees(Angle);
            Child?.Draw(availableSpace);
            skiaCanvas.SetMatrix(currentMatrix);
        }
    }
}