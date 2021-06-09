
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Translate : ContainerElement
    {
        public float TranslateX { get; set; } = 1;
        public float TranslateY { get; set; } = 1;

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var skiaCanvas = (canvas as Drawing.Canvas)?.SkiaCanvas;
            
            if (skiaCanvas == null)
                return;
            
            skiaCanvas.Translate(TranslateX, TranslateY);
            base.Draw(canvas, availableSpace);
            skiaCanvas.Translate(-TranslateX, -TranslateY);
        }
    }
}