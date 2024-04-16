using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class ScaleToFit : ContainerElement
    {
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (Child == null)
                return SpacePlan.None();

            var childMeasurementWithAvailableSpace = Child.Measure(availableSpace);
            
            if (childMeasurementWithAvailableSpace.Type == SpacePlanType.NoContent)
                return SpacePlan.None();
            
            var perfectScale = FindPerfectScale(Child, availableSpace);

            if (perfectScale == null)
                return SpacePlan.Wrap();

            var scaledSpace = ScaleSize(availableSpace, 1 / perfectScale.Value);
            var childSizeInScale = Child.Measure(scaledSpace);
            var childSizeInOriginalScale = ScaleSize(childSizeInScale, perfectScale.Value);
            return SpacePlan.FullRender(childSizeInOriginalScale);
        }
        
        internal override void Draw(Size availableSpace)
        {
            var perfectScale = FindPerfectScale(Child, availableSpace);
            
            if (!perfectScale.HasValue)
                return;

            var targetScale = perfectScale.Value;
            var targetSpace = ScaleSize(availableSpace, 1 / targetScale);
            
            Canvas.Scale(targetScale, targetScale);
            Child?.Draw(targetSpace);
            Canvas.Scale(1 / targetScale, 1 / targetScale);
        }

        private static Size ScaleSize(Size size, float factor)
        {
            return new Size(size.Width * factor, size.Height * factor);
        }
        
        private static float? FindPerfectScale(Element child, Size availableSpace)
        {
            if (ChildFits(1))
                return 1;
            
            var maxScale = 1f;
            var minScale = Size.Epsilon;

            var lastWorkingScale = (float?)null;
            
            foreach (var _ in Enumerable.Range(0, 8))
            {
                var halfScale = (maxScale + minScale) / 2;

                if (ChildFits(halfScale))
                {
                    minScale = halfScale;
                    lastWorkingScale = halfScale;
                }
                else
                {
                    maxScale = halfScale;
                }
            }
            
            return lastWorkingScale;
            
            bool ChildFits(float scale)
            {
                var scaledSpace = ScaleSize(availableSpace, 1 / scale);
                return child.Measure(scaledSpace).Type == SpacePlanType.FullRender;
            }
        }
    }
}