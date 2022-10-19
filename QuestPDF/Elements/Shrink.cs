using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Shrink : ContainerElement
    {
        public bool ShrinkVertical { get; set; }
        public bool ShrinkHorizontal { get; set; }
        
        internal override void Draw(Size availableSpace)
        {
            var childSize = base.Measure(availableSpace);

            if (childSize.Type == SpacePlanType.Wrap)
                return;
            
            var targetSize = new Size(
                ShrinkVertical ? childSize.Width : availableSpace.Width,
                ShrinkHorizontal ? childSize.Height : availableSpace.Height);
            
            // TODO: adjust offset for RTL mode
            
            base.Draw(targetSize);
        }
    }
}