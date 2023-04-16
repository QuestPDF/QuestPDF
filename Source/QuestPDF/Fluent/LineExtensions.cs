using System;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class LineExtensions
    {
        private static ILine Line(this IContainer element, LineType type, float size)
        {
            var line = new Line
            {
                Size = size,
                Type = type
            };

            element.Element(line);
            return line;
        }
        
        public static ILine LineVertical(this IContainer element, float size, Unit unit = Unit.Point)
        {
            return element.Line(LineType.Vertical, size.ToPoints(unit));
        }
        
        public static ILine LineHorizontal(this IContainer element, float size, Unit unit = Unit.Point)
        {
            return element.Line(LineType.Horizontal, size.ToPoints(unit));
        }
        
        public static void LineColor(this ILine descriptor, string value)
        {
            ColorValidator.Validate(value);
            (descriptor as Line).Color = value;
        }
    }
}