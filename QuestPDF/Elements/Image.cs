using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class Image : Element, IVisual, ICacheable
    {
        public bool IsRendered { get; set; }
        public bool RepeatContent { get; set; }
        
        public SKImage? InternalImage { get; set; }

        ~Image()
        {
            InternalImage?.Dispose();
        }
        
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
            if (InternalImage == null)
                return;

            IsRendered = true;
            Canvas.DrawImage(InternalImage, Position.Zero, availableSpace);
        }
    }
}