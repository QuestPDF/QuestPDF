using QuestPDF.Drawing;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace QuestPDF.Elements
{
    internal class ObjectElement : Element, ICacheable
    {
        public object? Object { get; set; }
        public AspectRatioOption SpaceFitBehavior { get; set; }

        internal override SpacePlan Measure(Size availableSpace)
        {
            return SpacePlan.FullRender(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            if (Object == null)
                return;

            Canvas.DrawObject(Object, Position.Zero, availableSpace);
        }
    }
}