using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class PaddingExtensions
    { 
        private static IContainer Padding(this IContainer element, Action<Padding> handler)
        {
            var padding = element as Padding ?? new Padding();
            handler(padding);
            
            return element.Element(padding);
        }
        
        public static IContainer Padding(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .PaddingVertical(value, unit)
                .PaddingHorizontal(value, unit);
        }
        
        public static IContainer PaddingHorizontal(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .PaddingLeft(value, unit)
                .PaddingRight(value, unit);
        }
        
        public static IContainer PaddingVertical(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .PaddingTop(value, unit)
                .PaddingBottom(value, unit);
        }
        
        public static IContainer PaddingTop(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Padding(x => x.Top += value.ToPoints(unit));
        }
        
        public static IContainer PaddingBottom(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Padding(x => x.Bottom += value.ToPoints(unit));
        }
        
        public static IContainer PaddingLeft(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Padding(x => x.Left += value.ToPoints(unit));
        }
        
        public static IContainer PaddingRight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Padding(x => x.Right += value.ToPoints(unit));
        }
    }
}