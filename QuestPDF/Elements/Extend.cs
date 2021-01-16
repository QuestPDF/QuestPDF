using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Extend : ContainerElement
    {
        public bool ExtendVertical { get; set; }
        public bool ExtendHorizontal { get; set; }
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            var childSize = Child?.Measure(availableSpace) ?? new FullRender(Size.Zero);

            if (childSize is Wrap)
                return childSize;
            
            var targetSize = GetTargetSize(availableSpace, childSize as Size);
            
            if (childSize is PartialRender)
                return new PartialRender(targetSize);
            
            if (childSize is FullRender)
                return new FullRender(targetSize);
            
            throw new NotSupportedException();
        }

        private Size GetTargetSize(Size availableSpace, Size childSize)
        {
            return new Size(
                ExtendHorizontal ? availableSpace.Width : childSize.Width, 
                ExtendVertical ? availableSpace.Height : childSize.Height);
        }
    }
}