using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class RelativePaddingExtensions
    {
        private static IContainer RelativePadding(this IContainer element, Action<RelativePadding> handler)
        {
            var relativePadding = element as RelativePadding ?? new RelativePadding();
            handler(relativePadding);
            
            return element.Element(relativePadding);
        }

        public static IContainer RelativePaddingTop(this IContainer element, float value)
        {
            return element.RelativePadding(x => x.Top += value);
        }
        
        public static IContainer RelativePaddingBottom(this IContainer element, float value)
        {
            return element.RelativePadding(x => x.Bottom += value);
        }
        
        public static IContainer RelativePaddingLeft(this IContainer element, float value)
        {
            return element.RelativePadding(x => x.Left += value);
        }
        
        public static IContainer RelativePaddingRight(this IContainer element, float value)
        {
            return element.RelativePadding(x => x.Right += value);
        }
    }
}