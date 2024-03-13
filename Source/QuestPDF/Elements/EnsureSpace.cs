using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class EnsureSpace : ContainerElement, ICacheable
    {
        public const float DefaultMinHeight = 150;
        public float MinHeight { get; set; } = DefaultMinHeight;

        internal override SpacePlan Measure(Size availableSpace)
        {
            var measurement = base.Measure(availableSpace);

            return measurement.Type switch
            {
                SpacePlanType.PartialRender when availableSpace.Height < MinHeight => SpacePlan.Wrap(),
                _ => measurement,
            };
        }
    }
}