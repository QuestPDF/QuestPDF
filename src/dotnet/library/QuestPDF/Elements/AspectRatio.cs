using System;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class AspectRatio : ContainerElement, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        public float Ratio { get; set; }
        public AspectRatioOption Option { get; set; } = AspectRatioOption.FitWidth;
        
        internal override SpacePlan Measure(LayoutSpace availableSpace)
        {
            if (Ratio == 0)
                return SpacePlan.FullRender(0, 0);
 
            if (Child.IsEmpty())
                return SpacePlan.Empty();
            
            if (availableSpace.IsCloseToZero())
                return SpacePlan.Wrap("The available space is zero.");
            
            var targetSpace = GetTargetSpace(availableSpace);
            var targetSize = targetSpace.Size;
            
            if (targetSize.Height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap("To preserve the target aspect ratio, the content requires more vertical space than available.");
            
            if (targetSize.Width > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap("To preserve the target aspect ratio, the content requires more horizontal space than available.");

            var childSize = base.Measure(targetSpace);

            if (childSize.Type == SpacePlanType.Wrap)
                return childSize;

            if (childSize.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(targetSize);

            if (childSize.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(targetSize);
            
            throw new NotSupportedException();
        }

        internal override void Draw(LayoutSpace availableSpace)
        {
            var targetSpace = GetTargetSpace(availableSpace);
            var size = targetSpace.Size;
            
            var offset = ContentDirection == ContentDirection.LeftToRight
                ? Position.Zero
                : new Position(availableSpace.Width - size.Width, 0);
            
            Canvas.Translate(offset);
            base.Draw(targetSpace);
            Canvas.Translate(offset.Reverse());
        }
        
        /// <summary>
        /// Determines the area given to the content, together with what that area means to it.
        /// </summary>
        /// <remarks>
        /// The area is always derived from a single axis of the available space, e.g. the FitWidth option takes
        /// the entire width and computes the height from it. The content is therefore constrained along both
        /// of its axes exactly when that source axis is constrained: if more space may still arrive along it,
        /// the whole area may grow.
        /// </remarks>
        private LayoutSpace GetTargetSpace(LayoutSpace availableSpace)
        {
            if (Ratio == 0)
                return availableSpace;
            
            var spaceRatio = availableSpace.Width / availableSpace.Height;

            var fitHeight = new Size(availableSpace.Height * Ratio, availableSpace.Height);
            var fitWidth = new Size(availableSpace.Width, availableSpace.Width / Ratio);

            var isDerivedFromWidth = Option switch
            {
                AspectRatioOption.FitWidth => true,
                AspectRatioOption.FitHeight => false,
                AspectRatioOption.FitArea => Ratio >= spaceRatio,
                _ => throw new ArgumentOutOfRangeException()
            };

            // The configured option requires more horizontal space than the environment provides.
            // Along a constrained axis this can never change: no later page is wider, and neither is any
            // other column of the same layout. The largest area of the desired ratio that does fit is the one
            // derived from the width, therefore it is used instead.
            //
            // Along an unconstrained axis the shortfall is reported as usual: a measurement asking about
            // the natural size has to describe the element exactly as configured.
            if (!isDerivedFromWidth && availableSpace.IsWidthFinal && fitHeight.Width > availableSpace.Width + Size.Epsilon)
                isDerivedFromWidth = true;

            return isDerivedFromWidth
                ? DerivedFromAxis(fitWidth, availableSpace.WidthMode)
                : DerivedFromAxis(fitHeight, availableSpace.HeightMode);

            static LayoutSpace DerivedFromAxis(Size size, LayoutAxisMode sourceAxisMode)
            {
                return new LayoutSpace(size, sourceAxisMode, sourceAxisMode);
            }
        }

        internal override string? GetCompanionHint() => $"{Option.ToString()} with ratio {Ratio:F1}";
    }
}