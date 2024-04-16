using System;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Padding : ContainerElement, ICacheable
    {
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }

        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Child == null)
                return SpacePlan.None();
            
            var internalSpace = InternalSpace(availableSpace);

            if (internalSpace.IsNegative())
                return SpacePlan.Wrap();
            
            var measure = base.Measure(internalSpace);

            if (measure.Type is SpacePlanType.NoContent or SpacePlanType.Wrap)
                return measure;

            var newSize = new Size(
                measure.Width + Left + Right,
                measure.Height + Top + Bottom);
            
            if (measure.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(newSize);
            
            if (measure.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(newSize);
            
            throw new NotSupportedException();
        }

        internal override void Draw(Size availableSpace)
        {
            if (Child == null)
                return;

            var internalSpace = InternalSpace(availableSpace);
            
            Canvas.Translate(new Position(Left, Top));
            base.Draw(internalSpace);
            Canvas.Translate(new Position(-Left, -Top));
        }

        private Size InternalSpace(Size availableSpace)
        {
            return new Size(
                availableSpace.Width - Left - Right,
                availableSpace.Height - Top - Bottom);
        }
        
        public override string ToString()
        {
            return $"Padding: Top({Top}) Right({Right}) Bottom({Bottom}) Left({Left})";
        }
    }
}