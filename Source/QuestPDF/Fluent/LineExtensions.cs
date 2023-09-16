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
        
        /// <summary>
        /// Renders a vertical line with a specified thickness.
        /// <a href="https://www.questpdf.com/api-reference/line.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// The line is not just a visual element; it occupies actual space within the document.
        /// </remarks>
        /// <returns>A descriptor to modify line attributes.</returns>
        public static ILine LineVertical(this IContainer element, float size, Unit unit = Unit.Point)
        {
            return element.Line(LineType.Vertical, size.ToPoints(unit));
        }
        
        /// <summary>
        /// Renders a horizontal line with a specified thickness.
        /// <a href="https://www.questpdf.com/api-reference/line.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// The line is not just a visual element; it occupies actual space within the document.
        /// </remarks>
        /// <returns>A descriptor to modify line attributes.</returns>
        public static ILine LineHorizontal(this IContainer element, float size, Unit unit = Unit.Point)
        {
            return element.Line(LineType.Horizontal, size.ToPoints(unit));
        }
        
        /// <summary>
        /// Specifies the color for the line.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static void LineColor(this ILine descriptor, string color)
        {
            ColorValidator.Validate(color);
            (descriptor as Line).Color = color;
        }
    }
}