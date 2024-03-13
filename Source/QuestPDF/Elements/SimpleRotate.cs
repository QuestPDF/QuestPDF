using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class SimpleRotate : ContainerElement, ICacheable
    {
        public int TurnCount { get; set; }
        public int NormalizedTurnCount => (TurnCount % 4 + 4) % 4;
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            switch (NormalizedTurnCount)
            {
                case 0:
                case 2:
                    return base.Measure(availableSpace);
            }

            availableSpace = new Size(availableSpace.Height, availableSpace.Width);
            var childSpace = base.Measure(availableSpace);

            if (childSpace.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();

            var targetSpace = new Size(childSpace.Height, childSpace.Width);

            return childSpace.Type switch
            {
                SpacePlanType.FullRender => SpacePlan.FullRender(targetSpace),
                SpacePlanType.PartialRender => SpacePlan.PartialRender(targetSpace),
                _ => throw new ArgumentException(),
            };
        }
        
        internal override void Draw(Size availableSpace)
        {
            var translate = new Position(
                (NormalizedTurnCount == 1 || NormalizedTurnCount == 2) ? availableSpace.Width : 0,
                (NormalizedTurnCount == 2 || NormalizedTurnCount == 3) ? availableSpace.Height : 0);

            var rotate = NormalizedTurnCount * 90;
            
            Canvas.Translate(translate);
            Canvas.Rotate(rotate);

            switch (NormalizedTurnCount)
            {
                case 1:
                case 3:
                    availableSpace = new Size(availableSpace.Height, availableSpace.Width);
                    break;
            }

            Child?.Draw(availableSpace);
            
            Canvas.Rotate(-rotate);
            Canvas.Translate(translate.Reverse());
        }
    }
}