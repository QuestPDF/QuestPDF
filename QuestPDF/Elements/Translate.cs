
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Translate : ContainerElement
    {
        public float TranslateX { get; set; } = 0;
        public float TranslateY { get; set; } = 0;

        internal override void Draw(Size availableSpace)
        {
            var skiaCanvas = (Canvas as Drawing.SkiaCanvasBase)?.Canvas;
            
            if (skiaCanvas == null)
                return;
            
            skiaCanvas.Translate(TranslateX, TranslateY);
            base.Draw(availableSpace);
            skiaCanvas.Translate(-TranslateX, -TranslateY);
        }
    }
}