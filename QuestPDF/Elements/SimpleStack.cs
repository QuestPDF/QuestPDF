using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class SimpleStack : Element
    {
        internal Element Current { get; set; } = Empty.Instance;
        internal Element Next { get; set; } = Empty.Instance;

        private bool IsFirstRendered { get; set; } = false;
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            var firstElement = IsFirstRendered ? Empty.Instance : Current;
            var firstSize = firstElement.Measure(availableSpace) as Size;

            if (firstSize == null)
                return new Wrap();
            
            if (firstSize is PartialRender partialRender)
                return partialRender;
                
            var spaceForSecond = new Size(availableSpace.Width, availableSpace.Height - firstSize.Height);
                
            var secondSize = Next.Measure(spaceForSecond) as Size;

            if (secondSize == null)
                return new PartialRender(firstSize);

            var totalWidth = Math.Max(firstSize.Width, secondSize.Width);
            var totalHeight = firstSize.Height + secondSize.Height;

            var targetSize = new Size(totalWidth, totalHeight);

            if (secondSize is PartialRender)
                return new PartialRender(targetSize);
                
            return new FullRender(targetSize);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var firstElement = IsFirstRendered ? Empty.Instance : Current;

            var firstMeasurement = firstElement.Measure(availableSpace);

            if (firstMeasurement is FullRender)
                IsFirstRendered = true;

            var firstSize = firstMeasurement as Size;

            if (firstSize != null)
                firstElement.Draw(canvas, firstSize);

            var firstHeight = firstSize?.Height ?? 0;

            var spaceForSecond = new Size(availableSpace.Width, availableSpace.Height - firstHeight);

            var secondMeasurement = Next?.Measure(spaceForSecond);

            if (secondMeasurement is Wrap)
                return;

            canvas.Translate(new Position(0, firstHeight));
            Next.Draw(canvas, secondMeasurement as Size);
            canvas.Translate(new Position(0, -firstHeight));
        }
    }
}