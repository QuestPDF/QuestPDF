using QuestPDF.Elements;
using QuestPDF.Helpers;
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
        
        /// <summary>
        /// Renders a vertical line with a specified thickness.
        /// <a href="https://www.questpdf.com/api-reference/line.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// The line is not just a visual element; it occupies actual space within the document.
        /// </remarks>
        /// <returns>A descriptor to modify line attributes.</returns>
        public static ILine LineVertical(this IContainer element, float thickess, Unit unit = Unit.Point)
        {
            return element.Line(LineType.Vertical, thickess.ToPoints(unit));
        }
        
        /// <summary>
        /// Renders a horizontal line with a specified thickness.
        /// <a href="https://www.questpdf.com/api-reference/line.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// The line is not just a visual element; it occupies actual space within the document.
        /// </remarks>
        /// <returns>A descriptor to modify line attributes.</returns>
        public static ILine LineHorizontal(this IContainer element, float thickness, Unit unit = Unit.Point)
        {
            return element.Line(LineType.Horizontal, thickness.ToPoints(unit));
        }
        
        /// <summary>
        /// Specifies the color for the line.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static void LineColor(this ILine descriptor, Color color)
        {
            (descriptor as Line).Color = color;
        }
    }
}