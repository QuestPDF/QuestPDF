using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class Image : Element
    {
        public SKImage? InternalImage { get; set; }

        ~Image()
        {
            InternalImage?.Dispose();
        }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            return new FullRender(availableSpace);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            if (InternalImage == null)
                return;

            canvas.DrawImage(InternalImage, Position.Zero, availableSpace);
        }
    }
}