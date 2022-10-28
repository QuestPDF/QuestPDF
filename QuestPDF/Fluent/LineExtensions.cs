using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class LineExtensions
    {
        private static ILine Line(this IContainer element, LineType type, float thickness)
        {
            var line = new Line
            {
                Thickness = thickness,
                Type = type
            };

            element.Element(line);
            return line;
        }
        
        public static ILine LineVertical(this IContainer element, float thickness, Unit unit = Unit.Point)
        {
            return element.Line(LineType.Vertical, thickness.ToPoints(unit));
        }
        
        public static ILine LineHorizontal(this IContainer element, float thickness, Unit unit = Unit.Point)
        {
            return element.Line(LineType.Horizontal, thickness.ToPoints(unit));
        }
        
        public static void LineColor(this ILine descriptor, string value)
        {
            (descriptor as Line).Color = value;
        }
    }
}