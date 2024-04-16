using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Alignment : ContainerElement
    {
        public VerticalAlignment? Vertical { get; set; }
        public HorizontalAlignment? Horizontal { get; set; }
        
        internal override void Draw(Size availableSpace)
        {
            if (Child == null)
                return;
            
            var childMeasurement = base.Measure(availableSpace);
            
            if (childMeasurement.Type is SpacePlanType.NoContent or SpacePlanType.Wrap)
                return;

            var childSize = new Size(
                Horizontal.HasValue ? childMeasurement.Width : availableSpace.Width,
                Vertical.HasValue ? childMeasurement.Height : availableSpace.Height);

            var top = GetTopOffset(availableSpace, childSize);
            var left = GetLeftOffset(availableSpace, childSize);
            
            Canvas.Translate(new Position(left, top));
            base.Draw(childSize);
            Canvas.Translate(new Position(-left, -top));
        }
        
        private float GetTopOffset(Size availableSpace, Size childSize)
        {
            var difference = availableSpace.Height - childSize.Height;

            return Vertical switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Middle => difference / 2,
                VerticalAlignment.Bottom => difference,
                _ => 0
            };
        }
        
        private float GetLeftOffset(Size availableSpace, Size childSize)
        {
            var difference = availableSpace.Width - childSize.Width;

            return Horizontal switch
            {
                HorizontalAlignment.Left => 0,
                HorizontalAlignment.Center => difference / 2,
                HorizontalAlignment.Right => difference,
                _ => 0
            };
        }
    }
}