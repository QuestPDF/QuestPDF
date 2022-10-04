using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class RelativePosition : ContainerElement
    {
        public float VerticalParent { get; set; }
        public float VerticalSelf { get; set; }
        
        public float HorizontalParent { get; set; }
        public float HorizontalSelf { get; set; } 
        
        internal override void Draw(Size availableSpace)
        {
            if (Child == null)
                return;
            
            var childSize = base.Measure(availableSpace);
            
            if (childSize.Type == SpacePlanType.Wrap)
                return;
            
            var left = availableSpace.Width * HorizontalParent + childSize.Width * HorizontalSelf;
            var top = availableSpace.Height * VerticalParent + childSize.Height * VerticalSelf;

            Canvas.Translate(new Position(left, top));
            base.Draw(childSize);
            Canvas.Translate(new Position(-left, -top));
        }
    }
}