using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Constrained : ContainerElement, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        public float MinWidth { get; set; }
        public float MaxWidth { get; set; } = float.PositiveInfinity;
        public float MinHeight { get; set; }
        public float MaxHeight { get; set; } = float.PositiveInfinity;

        internal bool HasWidthConstraint => MinWidth > 0 || !float.IsPositiveInfinity(MaxWidth);
        internal bool HasHeightConstraint => MinHeight > 0 || !float.IsPositiveInfinity(MaxHeight);

        public bool EnforceSizeWhenEmpty { get; set; }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (MinWidth > MaxWidth)
                return SpacePlan.Wrap($"The minimum width {MinWidth} is greater than the maximum width {MaxWidth}.");
            
            if (MinHeight > MaxHeight)
                return SpacePlan.Wrap($"The minimum height {MinHeight} is greater than the maximum height {MaxHeight}.");
            
            if (!EnforceSizeWhenEmpty && Child.IsEmpty())
                return SpacePlan.Empty();
            
            if (MinWidth > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap("The available horizontal space is less than the minimum width.");
            
            if (MinHeight > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap("The available vertical space is less than the minimum height.");
            
            var available = new Size(
                Math.Min(MaxWidth, availableSpace.Width),
                Math.Min(MaxHeight, availableSpace.Height));

            var measurement = base.Measure(available);

            if (measurement.Type == SpacePlanType.Wrap)
                return measurement;
            
            var actualSize = new Size(
                Math.Max(MinWidth, measurement.Width),
                Math.Max(MinHeight, measurement.Height));

            return measurement.Type switch
            {
                SpacePlanType.Empty when EnforceSizeWhenEmpty => SpacePlan.FullRender(actualSize),
                SpacePlanType.Empty => SpacePlan.Empty(),
                SpacePlanType.FullRender => SpacePlan.FullRender(actualSize),
                SpacePlanType.PartialRender => SpacePlan.PartialRender(actualSize),
                _ => throw new NotSupportedException()
            };
        }

        internal override void Draw(Size availableSpace)
        {
            var size = new Size(
                Math.Min(MaxWidth, availableSpace.Width),
                Math.Min(MaxHeight, availableSpace.Height));
            
            var offset = ContentDirection == ContentDirection.LeftToRight
                ? Position.Zero
                : new Position(availableSpace.Width - size.Width, 0);
            
            Canvas.Translate(offset);
            base.Draw(size);
            Canvas.Translate(offset.Reverse());
        }

        internal override string? GetCompanionHint()
        {
            var width = FormatRange("W", MinWidth, MaxWidth);
            var height = FormatRange("H", MinHeight, MaxHeight);
            
            return string.Join("   ", width.Concat(height));

            static IEnumerable<string> FormatRange(string prefix, float min, float max)
            {
                var hasMin = min > 0;
                var hasMax = !float.IsPositiveInfinity(max);
                
                if (!hasMin && !hasMax)
                    yield break;

                if (min == max)
                {
                    yield return $"{prefix}={min:F1}";
                    yield break;
                }

                if (hasMin)
                    yield return $"{prefix}≥{min:F1}";

                if (hasMax)
                    yield return $"{prefix}≤{max:F1}";
            }
        }
    }
}
