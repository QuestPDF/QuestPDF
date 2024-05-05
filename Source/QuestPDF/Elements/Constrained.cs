using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Constrained : ContainerElement, ICacheable, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        public float? MinWidth { get; set; }
        public float? MaxWidth { get; set; }

        public float? MinHeight { get; set; }
        public float? MaxHeight { get; set; }

        internal override SpacePlan Measure(Size availableSpace)
        {
            // TODO: disable this check when available space is close to Max
            // this will reduce cost of text measurement and its caching
            
            var measurementWithAllSpace = base.Measure(availableSpace);
            
            if (measurementWithAllSpace.Type is SpacePlanType.NoContent)
                return SpacePlan.None();
            
            if (MinWidth > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap();
            
            if (MinHeight > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap();
             
            var available = new Size(
                Min(MaxWidth, availableSpace.Width),
                Min(MaxHeight, availableSpace.Height));

            var measurement = base.Measure(available);

            if (measurement.Type is SpacePlanType.NoContent or SpacePlanType.Wrap)
                return measurement;
            
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
            var size = new Size(
                Min(MaxWidth, availableSpace.Width),
                Min(MaxHeight, availableSpace.Height));
            
            var offset = ContentDirection == ContentDirection.LeftToRight
                ? Position.Zero
                : new Position(availableSpace.Width - size.Width, 0);
            
            Canvas.Translate(offset);
            base.Draw(size);
            Canvas.Translate(offset.Reverse());
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