﻿using System;
using System.Text;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class SimpleRotate : ContainerElement
    {
        public int TurnCount { get; set; }
        public int NormalizedTurnCount => (TurnCount % 4 + 4) % 4;
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (NormalizedTurnCount == 0 || NormalizedTurnCount == 2)
                return base.Measure(availableSpace);
            
            availableSpace = new Size(availableSpace.Height, availableSpace.Width);
            var childSpace = base.Measure(availableSpace);

            if (childSpace.Type is SpacePlanType.Empty or SpacePlanType.Wrap)
                return childSpace;

            var targetSpace = new Size(childSpace.Height, childSpace.Width);

            if (childSpace.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(targetSpace);
            
            if (childSpace.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(targetSpace);

            // Stryker disable once: unreachable code
            throw new ArgumentException();
        }
        
        internal override void Draw(Size availableSpace)
        {
            var translate = new Position(
                (NormalizedTurnCount == 1 || NormalizedTurnCount == 2) ? availableSpace.Width : 0,
                (NormalizedTurnCount == 2 || NormalizedTurnCount == 3) ? availableSpace.Height : 0);

            var rotate = NormalizedTurnCount * 90;
            
            Canvas.Translate(translate);
            Canvas.Rotate(rotate);
            
            if (NormalizedTurnCount == 1 || NormalizedTurnCount == 3)
                availableSpace = new Size(availableSpace.Height, availableSpace.Width);
            
            Child?.Draw(availableSpace);
            
            Canvas.Rotate(-rotate);
            Canvas.Translate(translate.Reverse());
        }

        internal override string? GetCompanionHint()
        {
            if (TurnCount == 0)
                return "No rotation";

            var degrees = Math.Abs(TurnCount) * 90;
            
            // Stryker disable once equality: TurnCount = 0 is handled above
            var direction = TurnCount > 0 ? "clockwise" : "counter-clockwise"; 
            
            return $"{degrees} deg {direction}";
        }
    }
}