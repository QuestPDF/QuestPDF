using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Padding : ContainerElement
    {
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }

        internal override SpacePlan Measure(Size availableSpace)
        {
            var internalSpace = InternalSpace(availableSpace);

            if (internalSpace.IsNegative())
                return Child.IsEmpty() ? SpacePlan.Empty() : SpacePlan.Wrap("The available space is negative.");
            
            var measure = base.Measure(internalSpace);

            if (measure.Type is SpacePlanType.Empty or SpacePlanType.Wrap)
                return measure;

            var newWidth = Math.Max(0, measure.Width + Left + Right);
            var newHeight = Math.Max(0, measure.Height + Top + Bottom);
            
            var newSize = new Size(
                newWidth,
                newHeight);
            
            if (measure.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(newSize);
            
            return SpacePlan.FullRender(newSize);
        }

        internal override void Draw(Size availableSpace)
        {
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
        
        internal override string? GetCompanionHint()
        {
            return string.Join("   ", GetOptions().Where(x => x.value != 0).Select(x => $"{x.Label}={x.value:0.#}"));
            
            IEnumerable<(string Label, float value)> GetOptions()
            {
                if (Top == Bottom && Right == Left && Top == Right)
                {
                    yield return ("A", Top);
                    yield break;
                }

                if (Top == Bottom && Right == Left)
                {
                    yield return ("V", Top);
                    yield return ("H", Left);
                    yield break;
                }
                
                yield return ("L", Left);
                yield return ("T", Top);
                yield return ("R", Right);
                yield return ("B", Bottom);
            }
        }
    }
}