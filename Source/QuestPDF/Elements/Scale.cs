using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Scale : ContainerElement
    {
        public float ScaleX { get; set; } = 1;
        public float ScaleY { get; set; } = 1;
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var targetSpace = new Size(
                Math.Abs(availableSpace.Width / ScaleX), 
                Math.Abs(availableSpace.Height / ScaleY));
            
            var measure = base.Measure(targetSpace);

            if (measure.Type is SpacePlanType.Empty or SpacePlanType.Wrap)
                return measure;

            var targetSize = new Size(
                Math.Abs(measure.Width * ScaleX), 
                Math.Abs(measure.Height * ScaleY));

            if (measure.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(targetSize);
            
            if (measure.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(targetSize);
            
            // Stryker disable once: unreachable code
            throw new ArgumentException();
        }
        
        internal override void Draw(Size availableSpace)
        {
            var targetSpace = new Size(
                Math.Abs(availableSpace.Width / ScaleX), 
                Math.Abs(availableSpace.Height / ScaleY));

            var translate = new Position(
                ScaleX < 0 ? availableSpace.Width : 0,
                ScaleY < 0 ? availableSpace.Height : 0);
            
            Canvas.Translate(translate);
            Canvas.Scale(ScaleX, ScaleY);
            
            Child?.Draw(targetSpace);
             
            Canvas.Scale(1/ScaleX, 1/ScaleY);
            Canvas.Translate(translate.Reverse());
        }
        
        internal override string? GetCompanionHint()
        {
            return string.Join("   ", GetOptions().Where(x => x.value != 1).Select(x => $"{x.Label}={x.value.FormatAsCompanionNumber()}"));
            
            IEnumerable<(string Label, float value)> GetOptions()
            {
                if (ScaleX == ScaleY)
                {
                    yield return ("A", ScaleX);
                    yield break;
                }
                
                yield return ("H", ScaleX);
                yield return ("V", ScaleY);
            }
        }
    }
}