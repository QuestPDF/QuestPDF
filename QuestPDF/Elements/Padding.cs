using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Padding : ContainerElement, ICacheable, ICollectable
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
        
        public override void Collect()
        {
            base.Collect();
            
            Left = 0;
            Right = 0;
            Bottom = 0;
            Top = 0;
        }
    }
}