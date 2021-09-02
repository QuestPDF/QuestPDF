using System;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Constrained : ContainerElement
    {
        public float? MinWidth { get; set; }
        public float? MaxWidth { get; set; }

        public float? MinHeight { get; set; }
        public float? MaxHeight { get; set; }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (MinWidth > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap();
            
            if (MinHeight > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap();
            
            var available = new Size(
                LimitMin(availableSpace.Width, MaxWidth),
                LimitMin(availableSpace.Height, MaxHeight));

            var measurement = base.Measure(available);

            if (measurement.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();
            
            var actualSize = new Size(
                LimitMax(measurement.Width, MinWidth),
                LimitMax(measurement.Height, MinHeight));
            
            if (measurement.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(actualSize);
            
            if (measurement.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(actualSize);
            
            throw new NotSupportedException();
        }
        
        internal override void Draw(Size availableSpace)
        {
            var available = new Size(
                LimitMin(availableSpace.Width, MaxWidth),
                LimitMin(availableSpace.Height, MaxHeight));
            
            base.Draw(available);
        }

        private float LimitMin(float value, float? limit)
        {
            return limit.HasValue ? Math.Min(value, limit.Value) : value;
        }
        
        private float LimitMax(float value, float? limit)
        {
            return limit.HasValue ? Math.Max(value, limit.Value) : value;
        }
    }
}