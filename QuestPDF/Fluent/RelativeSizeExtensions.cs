using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class RelativeSizeExtensions
    {
        private static IContainer RelativeSize(this IContainer element, Action<RelativeSize> handler)
        {
            var relativeSize = element as RelativeSize ?? new RelativeSize();
            handler(relativeSize);
            
            return element.Element(relativeSize);
        }

        public static IContainer RelativeWidth(this IContainer element, float value)
        {
            return element.RelativeSize(x => x.WidthFactor = value);
        }
        
        public static IContainer RelativeHeight(this IContainer element, float value)
        {
            return element.RelativeSize(x => x.HeightFactor = value);
        }
    }
}