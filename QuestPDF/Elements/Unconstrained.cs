using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Unconstrained : ContainerElement, ICacheable
    {
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
            
            if (measurement.Type == SpacePlanType.Wrap)
                return;

            base.Draw(measurement);
        }
    }
}