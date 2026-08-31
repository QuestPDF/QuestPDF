using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    /// <remarks>
    /// The scaled areas offered to the content are candidates in a search, not allocations. The content has to
    /// describe itself exactly as configured at every scale, otherwise it would adapt to the first candidate
    /// and the search would never shrink it. This element is the adaptation the author chose explicitly.
    /// </remarks>
    internal sealed class ScaleToFit : ContainerElement
    {
        internal override SpacePlan Measure(LayoutSpace availableSpace)
        {
            var perfectScale = FindPerfectScale(availableSpace);

            if (perfectScale == null)
                return SpacePlan.Wrap("Cannot find the perfect scale to fit the child element in the available space.");

            var scaledSpace = ScaleSize(availableSpace, 1 / perfectScale.Value);
            var childSizeInScale = base.Measure(LayoutSpace.Candidate(scaledSpace));
            var childSizeInOriginalScale = ScaleSize(childSizeInScale, perfectScale.Value);
            return SpacePlan.FullRender(childSizeInOriginalScale);
        }
        
        internal override void Draw(LayoutSpace availableSpace)
        {
            var perfectScale = FindPerfectScale(availableSpace);
            
            if (!perfectScale.HasValue)
                return;

            var targetScale = perfectScale.Value;
            var targetSpace = ScaleSize(availableSpace, 1 / targetScale);
            
            Canvas.Scale(targetScale, targetScale);
            Child?.Draw(LayoutSpace.Candidate(targetSpace));
            Canvas.Scale(1 / targetScale, 1 / targetScale);
        }

        private static Size ScaleSize(Size size, float factor)
        {
            return new Size(size.Width * factor, size.Height * factor);
        }
        
        private float? FindPerfectScale(Size availableSpace)
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
                return base.Measure(LayoutSpace.Candidate(scaledSpace)).Type is SpacePlanType.Empty or SpacePlanType.FullRender;
            }
        }
    }
}