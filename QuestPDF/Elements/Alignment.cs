using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Alignment : ContainerElement
    {
        public VerticalAlignment Vertical { get; set; } = VerticalAlignment.Top;
        public HorizontalAlignment Horizontal { get; set; } = HorizontalAlignment.Left;
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            if (Child == null)
                return;
            
            var childSize = Child.Measure(availableSpace) as Size;
            
            if (childSize == null)
                return;
            
            var top = GetTopOffset(availableSpace, childSize);
            var left = GetLeftOffset(availableSpace, childSize);
            
            canvas.Translate(new Position(left, top));
            Child.Draw(canvas, childSize);
            canvas.Translate(new Position(-left, -top));
        }
        
        private float GetTopOffset(Size availableSpace, Size childSize)
        {
            var difference = availableSpace.Height - childSize.Height;

            return Vertical switch
            {
                VerticalAlignment.Top => 0,
                VerticalAlignment.Middle => difference / 2,
                VerticalAlignment.Bottom => difference,
                _ => throw new NotSupportedException()
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
                _ => throw new NotSupportedException()
            };
        }
    }
}