using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class RelativeSize : ContainerElement
    {
        public float? WidthFactor { get; set; } = 1f;
        public float? HeightFactor { get; set; } = 1f;

        internal override SpacePlan Measure(Size availableSpace)
        {
            var internalSpace = new Size(
                availableSpace.Width * (WidthFactor ?? 1),
                availableSpace.Height * (HeightFactor ?? 1));
            
            var childSpace = Child?.Measure(internalSpace) ?? SpacePlan.FullRender(0, 0);

            if (childSpace.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();

            var targetSpace = new Size(
                WidthFactor.HasValue ? internalSpace.Width : childSpace.Width,
                HeightFactor.HasValue ? internalSpace.Height : childSpace.Height);
            
            if (childSpace.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(targetSpace);
            
            if (childSpace.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(targetSpace);

            throw new ArgumentException();
        }
    }
}