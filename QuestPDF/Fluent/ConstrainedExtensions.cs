using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ConstrainedExtensions
    {
        private static IContainer Constrained(this IContainer element, Action<Constrained> handler)
        {
            var constrained = element as Constrained ?? new Constrained();
            handler(constrained);
            
            return element.Element(constrained);
        }
        
        public static IContainer Width(this IContainer element, float value)
        {
            return element.MinWidth(value).MaxWidth(value);
        }
        
        public static IContainer MinWidth(this IContainer element, float value)
        {
            return element.Constrained(x => x.MinWidth = value);
        }
        
        public static IContainer MaxWidth(this IContainer element, float value)
        {
            return element.Constrained(x => x.MaxWidth = value);
        }
        
        public static IContainer Height(this IContainer element, float value)
        {
            return element.MinHeight(value).MaxHeight(value);
        }
        
        public static IContainer MinHeight(this IContainer element, float value)
        {
            return element.Constrained(x => x.MinHeight = value);
        }
        
        public static IContainer MaxHeight(this IContainer element, float value)
        {
            return element.Constrained(x => x.MaxHeight = value);
        }
    }
}