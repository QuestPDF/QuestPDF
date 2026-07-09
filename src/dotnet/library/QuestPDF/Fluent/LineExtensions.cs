using System;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class LineDescriptor
    {
        internal Line Line { get; } = new Line();

        internal LineDescriptor()
        {
            
        }
        
        /// <summary>
        /// Specifies the color for the line.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public LineDescriptor LineColor(Color color)
        {
            Line.Color = color;
            return this;
        }

        /// <summary>
        /// Configures a dashed pattern for the line.
        /// For example, a pattern of [2, 3] creates a dash of 2 units followed by a gap of 3 units.
        /// </summary>
        /// <param name="dashPattern">The length of this array must be even.</param>
        public LineDescriptor LineDashPattern(float[] dashPattern, Unit unit = Unit.Point)
        {
            if (dashPattern == null)
                throw new ArgumentNullException(nameof(dashPattern), "The dash pattern cannot be null.");
            
            if (dashPattern.Length == 0)
                throw new ArgumentException("The dash pattern cannot be empty.", nameof(dashPattern));
            
            if (dashPattern.Length % 2 != 0)
                throw new ArgumentException("The dash pattern must contain an even number of elements.", nameof(dashPattern));
            
            Line.DashPattern = dashPattern.Select(x => x.ToPoints(unit)).ToArray();
            return this;
        }

        /// <summary>
        /// Applies a linear gradient to a line using the specified colors.
        /// </summary>
        public LineDescriptor LineGradient(Color[] colors)
        {
            if (colors == null)
                throw new ArgumentNullException(nameof(colors), "The gradient colors cannot be null.");

            if (colors.Length == 0)
                throw new ArgumentException("The gradient colors cannot be empty.", nameof(colors));

            Line.GradientColors = colors;
            return this;
        }
    }
    
    public static class LineExtensions
    {
        private static LineDescriptor Line(this IContainer element, LineType type, float thickness)
        {
            if (thickness < 0)
                throw new ArgumentOutOfRangeException(nameof(thickness), "The Line thickness cannot be negative.");
            
            var descriptor = new LineDescriptor();
            descriptor.Line.Thickness = thickness;
            descriptor.Line.Type = type;
            
            element.Element(descriptor.Line);
            return descriptor;
        }
        
        /// <summary>
        /// Renders a vertical line with a specified thickness.
        /// <a href="https://www.questpdf.com/api-reference/line.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// The line is not just a visual element; it occupies actual space within the document.
        /// </remarks>
        /// <returns>A descriptor to modify line attributes.</returns>
        public static LineDescriptor LineVertical(this IContainer element, float thickness, Unit unit = Unit.Point)
        {
            return element.Line(LineType.Vertical, thickness.ToPoints(unit));
        }
        
        /// <summary>
        /// Renders a horizontal line with a specified thickness.
        /// <a href="https://www.questpdf.com/api-reference/line.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// The line is not just a visual element; it occupies actual space within the document.
        /// </remarks>
        /// <returns>A descriptor to modify line attributes.</returns>
        public static LineDescriptor LineHorizontal(this IContainer element, float thickness, Unit unit = Unit.Point)
        {
            return element.Line(LineType.Horizontal, thickness.ToPoints(unit));
        }
    }
}