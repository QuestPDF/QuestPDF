using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class RelativePosition : ContainerElement
    {
        public float VerticalParent { get; set; }
        public float VerticalChild { get; set; }
        
        public float HorizontalParent { get; set; }
        public float HorizontalChild { get; set; } 
        
        internal override void Draw(Size availableSpace)
        {
            if (Child == null)
                return;
            
            var childSize = base.Measure(availableSpace);
            
            if (childSize.Type == SpacePlanType.Wrap)
                return;
            
            var left = availableSpace.Width * HorizontalParent + childSize.Width * HorizontalChild;
            var top = availableSpace.Height * VerticalParent + childSize.Height * VerticalChild;

            Canvas.Translate(new Position(left, top));
            base.Draw(childSize);
            Canvas.Translate(new Position(-left, -top));
        }
    }
}