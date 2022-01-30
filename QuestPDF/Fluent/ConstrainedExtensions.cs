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
        
        public static IContainer Width(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .MinWidth(value, unit)
                .MaxWidth(value, unit);
        }
        
        public static IContainer MinWidth(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Constrained(x => x.MinWidth = value.ToPoints(unit));
        }
        
        public static IContainer MaxWidth(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Constrained(x => x.MaxWidth = value.ToPoints(unit));
        }
        
        public static IContainer Height(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .MinHeight(value, unit)
                .MaxHeight(value, unit);
        }
        
        public static IContainer MinHeight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Constrained(x => x.MinHeight = value.ToPoints(unit));
        }
        
        public static IContainer MaxHeight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Constrained(x => x.MaxHeight = value.ToPoints(unit));
        }
    }
}