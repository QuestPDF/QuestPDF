using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Unconstrained : ContainerElement, IContentDirectionAware, ICacheable
    {
        public ContentDirection ContentDirection { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var childSize = base.Measure(Size.Max);
            
            if (childSize.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(0, 0);
            
            if (childSize.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(0, 0);
            
            return childSize;
        }

        internal override void Draw(Size availableSpace)
        {
            var measurement = base.Measure(Size.Max);
            
            if (measurement.Type is SpacePlanType.NoContent or SpacePlanType.Wrap)
                return;

            var translate = ContentDirection == ContentDirection.RightToLeft
                ? new Position(-measurement.Width, 0)
                : Position.Zero;
            
            Canvas.Translate(translate);
            base.Draw(measurement);
            Canvas.Translate(translate.Reverse());
        }
    }
}