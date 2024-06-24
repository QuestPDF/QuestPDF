using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class EnsureSpace : ContainerElement
    {
        public const float DefaultMinHeight = 150;
        public float MinHeight { get; set; } = DefaultMinHeight;

        internal override SpacePlan Measure(Size availableSpace)
        {
            var measurement = base.Measure(availableSpace);

            if (measurement.Type == SpacePlanType.PartialRender && availableSpace.Height < MinHeight)
                return SpacePlan.Wrap("The available vertical space is smaller than requested in the constraint.");

            return measurement.Forward();
        }
    }
}