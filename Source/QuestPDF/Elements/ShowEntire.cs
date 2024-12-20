using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class ShowEntire : ContainerElement
    {
        internal override SpacePlan Measure(Size availableSpace)
        {
            var childMeasurement = base.Measure(availableSpace);
            
            if (childMeasurement.Type is SpacePlanType.Wrap)
                return SpacePlan.Wrap("Child element does not fit (even partially) on the page.");
            
            if (childMeasurement.Type is SpacePlanType.PartialRender)
                return SpacePlan.Wrap("Child element fits only partially on the page.");
            
            return childMeasurement;
        }
    }
}