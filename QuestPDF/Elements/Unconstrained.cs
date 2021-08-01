using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Unconstrained : ContainerElement
    {
        internal override ISpacePlan Measure(Size availableSpace)
        {
            var childSize = Child?.Measure(Size.Max) ?? new FullRender(Size.Zero);
            
            if (childSize is PartialRender)
                return new PartialRender(Size.Zero);
            
            if (childSize is FullRender)
                return new FullRender(Size.Zero);
            
            return childSize;
        }

        internal override void Draw(Size availableSpace)
        {
            Child?.Draw(Size.Max);
        }
    }
}