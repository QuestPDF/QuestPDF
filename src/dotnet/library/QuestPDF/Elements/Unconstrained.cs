using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Unconstrained : ContainerElement, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        // the content is asked about its natural size and has to describe itself exactly as configured
        internal override SpacePlan Measure(LayoutSpace availableSpace)
        {
            var childSize = base.Measure(LayoutSpace.Query(Size.Max));
            
            if (childSize.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(0, 0);
            
            if (childSize.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(0, 0);
            
            return childSize;
        }

        internal override void Draw(LayoutSpace availableSpace)
        {
            var measurement = base.Measure(LayoutSpace.Query(Size.Max));
            
            if (measurement.Type is SpacePlanType.Empty or SpacePlanType.Wrap)
                return;

            var translate = ContentDirection == ContentDirection.RightToLeft
                ? new Position(-measurement.Width, 0)
                : Position.Zero;
            
            Canvas.Translate(translate);
            base.Draw(LayoutSpace.Query(measurement));
            Canvas.Translate(translate.Reverse());
        }
    }
}