using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class SimpleRow : Element
    {
        internal Element Left { get; set; }
        internal Element Right { get; set; }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            var leftMeasurement = Left.Measure(new Size(availableSpace.Width, availableSpace.Height)) as Size;
            
            if (leftMeasurement == null)
                return new Wrap();
            
            var rightMeasurement = Right.Measure(new Size(availableSpace.Width - leftMeasurement.Width, availableSpace.Height)) as Size;

            if (rightMeasurement == null)
                return new Wrap();
            
            var totalWidth = leftMeasurement.Width + rightMeasurement.Width;
            var totalHeight = Math.Max(leftMeasurement.Height, rightMeasurement.Height);

            var targetSize = new Size(totalWidth, totalHeight);

            if (leftMeasurement is PartialRender || rightMeasurement is PartialRender)
                return new PartialRender(targetSize);
            
            return new FullRender(targetSize);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var leftMeasurement = Left.Measure(new Size(availableSpace.Width, availableSpace.Height));
            var leftWidth = (leftMeasurement as Size)?.Width ?? 0;
            
            Left.Draw(canvas, new Size(leftWidth, availableSpace.Height));
            
            canvas.Translate(new Position(leftWidth, 0));
            Right.Draw(canvas, new Size(availableSpace.Width - leftWidth, availableSpace.Height));
            canvas.Translate(new Position(-leftWidth, 0));
        }
    }
}