using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Shrink : ContainerElement, IContentDirectionAware
    {
        public bool Vertical { get; set; }
        public bool Horizontal { get; set; }
        
        public ContentDirection ContentDirection { get; set; }

        internal override void Draw(Size availableSpace)
        {
            var childSize = base.Measure(availableSpace);

            if (childSize.Type is SpacePlanType.Empty or SpacePlanType.Wrap)
                return;
            
            var targetSize = new Size(
                Horizontal ? childSize.Width : availableSpace.Width,
                Vertical ? childSize.Height : availableSpace.Height);

            var translate = ContentDirection == ContentDirection.RightToLeft
                ? new Position(availableSpace.Width - targetSize.Width, 0)
                : Position.Zero;

            Canvas.Translate(translate);
            base.Draw(targetSize);
            Canvas.Translate(translate.Reverse());
        }
    }
}