using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class ShowEntire : ContainerElement, ICacheable
    {
        internal override SpacePlan Measure(Size availableSpace)
        {
            var childMeasurement = base.Measure(availableSpace);

            return childMeasurement.Type switch
            {
                SpacePlanType.FullRender => childMeasurement,
                _ => SpacePlan.Wrap(),
            };
        }
    }
}