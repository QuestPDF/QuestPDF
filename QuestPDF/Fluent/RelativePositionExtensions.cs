using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class RelativePositionExtensions
    {
        private static IContainer RelativePosition(this IContainer element, Action<RelativePosition> handler)
        {
            var relativePosition = element as RelativePosition ?? new RelativePosition();
            handler(relativePosition);
            
            return element.Element(relativePosition);
        }

        public static IContainer RelativePositionVertical(this IContainer element, float parentOffset, float childOffset)
        {
            return element.RelativePosition(x =>
            {
                x.VerticalParent = parentOffset;
                x.VerticalChild = childOffset;
            });
        }
        
        public static IContainer RelativePositionHorizontal(this IContainer element, float parentOffset, float childOffset)
        {
            return element.RelativePosition(x =>
            {
                x.HorizontalParent = parentOffset;
                x.HorizontalChild = childOffset;
            });
        }
    }
}