using System;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Constrained : ContainerElement, ICacheable
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
                Min(MaxWidth, availableSpace.Width),
                Min(MaxHeight, availableSpace.Height));

            var measurement = base.Measure(available);

            if (measurement.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();
            
            var actualSize = new Size(
                Max(MinWidth, measurement.Width),
                Max(MinHeight, measurement.Height));
            
            if (measurement.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(actualSize);
            
            if (measurement.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(actualSize);
            
            throw new NotSupportedException();
        }
        
        internal override void Draw(Size availableSpace)
        {
            var available = new Size(
                Min(MaxWidth, availableSpace.Width),
                Min(MaxHeight, availableSpace.Height));
            
            Child?.Draw(available);
        }
        
        private static float Min(float? x, float y)
        {
            return x.HasValue ? Math.Min(x.Value, y) : y; 
        }
        
        private static float Max(float? x, float y)
        {
            return x.HasValue ? Math.Max(x.Value, y) : y;
        }
    }
}