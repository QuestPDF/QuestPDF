using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class ShowEntire : ContainerElement
    {
        internal override ISpacePlan Measure(Size availableSpace)
        {
            var childMeasurement = Child?.Measure(availableSpace) ?? new FullRender(Size.Zero);

            if (childMeasurement is FullRender)
                return childMeasurement;

            return new Wrap();
        }
    }
}