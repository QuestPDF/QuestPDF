using System;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class BorderExtensions
    {
        private static IContainer Border(this IContainer element, Action<Border> handler)
        {
            var border = element as Border ?? ElementCacheManager.Get<Border>();
            handler(border);
            
            return element.Element(border);
        }
        
        public static IContainer Border(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .BorderHorizontal(value, unit)
                .BorderVertical(value, unit);
        }
        
        public static IContainer BorderVertical(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .BorderLeft(value, unit)
                .BorderRight(value, unit);
        }
        
        public static IContainer BorderHorizontal(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .BorderTop(value, unit)
                .BorderBottom(value, unit);
        }
        
        public static IContainer BorderLeft(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.Left = value.ToPoints(unit));
        }
        
        public static IContainer BorderRight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.Right = value.ToPoints(unit));
        }
        
        public static IContainer BorderTop(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.Top = value.ToPoints(unit));
        }
        
        public static IContainer BorderBottom(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.Bottom = value.ToPoints(unit));
        }
        
        public static IContainer BorderColor(this IContainer element, string color)
        {
            return element.Border(x => x.Color = color);
        }
    }
}