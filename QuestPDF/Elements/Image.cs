using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class Image : Element, ICacheable, IContent
    {
        public SKImage? InternalImage { get; set; }

        ~Image()
        {
            InternalImage?.Dispose();
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return availableSpace.IsNegative() 
                ? SpacePlan.Wrap() 
                : SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (InternalImage == null)
                return;

            Canvas.DrawImage(InternalImage, Position.Zero, availableSpace);
        }
    }
}