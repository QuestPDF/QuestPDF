using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class ShowEntire : ContainerElement, ICacheable
    {
        internal override SpacePlan Measure(Size availableSpace)
        {
            var childMeasurement = base.Measure(availableSpace);

            if (childMeasurement.Type == SpacePlanType.FullRender)
                return childMeasurement;

            return SpacePlan.Wrap();
        }
    }
}