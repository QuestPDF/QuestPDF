﻿using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class StopPaging : ContainerElement
    {
        internal override SpacePlan Measure(Size availableSpace)
        {
            var measurement = base.Measure(availableSpace);

            return measurement.Type switch
            {
                SpacePlanType.Wrap => SpacePlan.FullRender(Size.Zero),
                SpacePlanType.PartialRender => SpacePlan.FullRender(measurement),
                SpacePlanType.FullRender => measurement,
                SpacePlanType.Empty => measurement,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        internal override void Draw(Size availableSpace)
        {
            var measurement = base.Measure(availableSpace);
            
            if (measurement.Type is SpacePlanType.Wrap)
                return;
                
            base.Draw(availableSpace);
        }
    }
}