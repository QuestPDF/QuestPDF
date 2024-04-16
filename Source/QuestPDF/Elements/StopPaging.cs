using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class StopPaging : ContainerElement
    {
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Child == null)
                return SpacePlan.None();

            var measurement = Child.Measure(availableSpace);

            return measurement.Type switch
            {
                SpacePlanType.NoContent => SpacePlan.None(),
                SpacePlanType.Wrap => SpacePlan.FullRender(Size.Zero),
                SpacePlanType.PartialRender => SpacePlan.FullRender(measurement),
                SpacePlanType.FullRender => measurement,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}