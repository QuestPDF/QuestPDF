using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class RelativePadding : ContainerElement
    {
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Child == null)
                return SpacePlan.FullRender(0, 0);
            
            var internalSpace = InternalSpace(availableSpace);

            if (internalSpace.Width < 0 || internalSpace.Height < 0)
                return SpacePlan.Wrap();
            
            var measure = base.Measure(internalSpace);

            if (measure.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();

            if (measure.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(availableSpace);
            
            if (measure.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(availableSpace);
            
            throw new NotSupportedException();
        }

        internal override void Draw(Size availableSpace)
        {
            if (Child == null)
                return;

            var internalOffset = InternalOffset(availableSpace);
            var internalSpace = InternalSpace(availableSpace);
            
            Canvas.Translate(internalOffset);
            base.Draw(internalSpace);
            Canvas.Translate(internalOffset.Reverse());
        }

        private Position InternalOffset(Size availableSpace)
        {
            return new Position(
                availableSpace.Width * Left,
                availableSpace.Height * Top);
        }
        
        private Size InternalSpace(Size availableSpace)
        {
            return new Size(
                availableSpace.Width * (1f - Left - Right),
                availableSpace.Height * (1f - Top - Bottom));
        }
    }
}