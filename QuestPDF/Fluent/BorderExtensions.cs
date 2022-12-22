using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class BorderExtensions
    {
        private static IContainer Border(this IContainer element, Action<Border> handler)
        {
            var border = element as Border ?? new Border();
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

        public static IContainer BorderCorners(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x =>
            {
                var v = value.ToPoints(unit);
                x.TopLeftCorner = v;
                x.TopRightCorner = v;
                x.BottomLeftCorner = v;
                x.BottomRightCorner = v;
            });
        }

        public static IContainer BorderTopLeftCorner(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.TopLeftCorner = value.ToPoints(unit));
        }
        
        public static IContainer BorderTopRightCorner(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.TopRightCorner = value.ToPoints(unit));
        }
        
        public static IContainer BorderBottomLeftCorner(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.BottomLeftCorner = value.ToPoints(unit));
        }
        
        public static IContainer BorderBottomRightCorner(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.BottomRightCorner = value.ToPoints(unit));
        }
    }
}