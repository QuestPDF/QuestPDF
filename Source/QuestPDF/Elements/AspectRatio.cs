using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class AspectRatio : ContainerElement, ICacheable, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        
        public float Ratio { get; set; }
        public AspectRatioOption Option { get; set; } = AspectRatioOption.FitWidth;
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Ratio == 0)
                return SpacePlan.FullRender(Size.Zero);
            
            if (Child == null)
                return SpacePlan.None();
            
            var targetSize = GetTargetSize(availableSpace);
            var childSize = base.Measure(targetSize);
            
            if (childSize.Type == SpacePlanType.NoContent)
                return SpacePlan.None();
            
            if (targetSize.Height > availableSpace.Height + Size.Epsilon)
                return SpacePlan.Wrap();
            
            if (targetSize.Width > availableSpace.Width + Size.Epsilon)
                return SpacePlan.Wrap();
            
            if (childSize.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap();

            if (childSize.Type == SpacePlanType.PartialRender)
                return SpacePlan.PartialRender(targetSize);

            if (childSize.Type == SpacePlanType.FullRender)
                return SpacePlan.FullRender(targetSize);
            
            throw new NotSupportedException();
        }

        internal override void Draw(Size availableSpace)
        {
            if (Child == null)
                return;
            
            var size = GetTargetSize(availableSpace);
            
            var offset = ContentDirection == ContentDirection.LeftToRight
                ? Position.Zero
                : new Position(availableSpace.Width - size.Width, 0);
            
            Canvas.Translate(offset);
            base.Draw(size);
            Canvas.Translate(offset.Reverse());
        }
        
        private Size GetTargetSize(Size availableSpace)
        {
            if (Ratio == 0)
                return availableSpace;
            
            var spaceRatio = availableSpace.Width / availableSpace.Height;

            var fitHeight = new Size(availableSpace.Height * Ratio, availableSpace.Height) ;
            var fitWidth = new Size(availableSpace.Width, availableSpace.Width / Ratio);

            return Option switch
            {
                AspectRatioOption.FitWidth => fitWidth,
                AspectRatioOption.FitHeight => fitHeight,
                AspectRatioOption.FitArea => Ratio < spaceRatio ? fitHeight : fitWidth,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}