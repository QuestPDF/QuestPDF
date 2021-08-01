using System;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Constrained : ContainerElement
    {
        public float? MinWidth { get; set; }
        public float? MaxWidth { get; set; }
        
        public float? MinHeight { get; set; }
        public float? MaxHeight { get; set; }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (MinWidth > availableSpace.Width + Size.Epsilon)
                return new Wrap();
            
            if (MinHeight > availableSpace.Height + Size.Epsilon)
                return new Wrap();
            
            var available = new Size(
                MathHelpers.Min(MaxWidth, availableSpace.Width),
                MathHelpers.Min(MaxHeight, availableSpace.Height));

            var measurement = Child?.Measure(available) ?? new FullRender(Size.Zero);
            var size = measurement as Size;

            if (measurement is Wrap)
                return new Wrap();
            
            var actualSize = new Size(
                MathHelpers.Max(MinWidth, size.Width),
                MathHelpers.Max(MinHeight, size.Height));
            
            if (size is FullRender)
                return new FullRender(actualSize);
            
            if (size is PartialRender)
                return new PartialRender(actualSize);
            
            throw new NotSupportedException();
        }
        
        internal override void Draw(Size availableSpace)
        {
            var available = new Size(
                MathHelpers.Min(MaxWidth, availableSpace.Width),
                MathHelpers.Min(MaxHeight, availableSpace.Height));
            
            Child?.Draw(available);
        }
    }
    
    static class MathHelpers
    {
        public static float Min(params float?[] values)
        {
            return values.Where(x => x.HasValue).Min().Value;
        }
        
        public static float Max(params float?[] values)
        {
            return values.Where(x => x.HasValue).Max().Value;
        }
    }
}