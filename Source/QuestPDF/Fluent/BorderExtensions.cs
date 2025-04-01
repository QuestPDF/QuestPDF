using System;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class BorderExtensions
    {
        private static IContainer Border(this IContainer element, float top = 0, float bottom = 0, float left = 0, float right = 0)
        {
            var border = element as Border ?? new Border();

            if (top < 0)
                throw new ArgumentOutOfRangeException(nameof(top), "The top border cannot be negative.");
            
            if (bottom < 0)
                throw new ArgumentOutOfRangeException(nameof(bottom), "The bottom border cannot be negative.");
            
            if (left < 0)
                throw new ArgumentOutOfRangeException(nameof(left), "The left border cannot be negative.");
            
            if (right < 0)
                throw new ArgumentOutOfRangeException(nameof(right), "The right border cannot be negative.");
            
            border.Top += top;
            border.Bottom += bottom;
            border.Left += left;
            border.Right += right;

            return element.Element(border);
        }
        
        /// <summary>
        /// Sets a uniform border (all edges) for its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer Border(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(top: value, bottom: value, left: value, right: value);
        }
        
        /// <summary>
        /// Sets a vertical border (left and right) for its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderVertical(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(left: value, right: value);
        }
        
        /// <summary>
        /// Sets a horizontal border (top and bottom) for its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderHorizontal(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(top: value, bottom: value);
        }
        
        /// <summary>
        /// Sets a border on the left side of its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderLeft(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(left: value);
        }
        
        /// <summary>
        /// Sets a border on the right side of its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderRight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(right: value);
        }
        
        /// <summary>
        /// Sets a border on the top side of its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderTop(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(top: value);
        }
        
        /// <summary>
        /// Sets a border on the bottom side of its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>        
        public static IContainer BorderBottom(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(bottom: value);
        }
        
        /// <summary>
        /// Adjusts color of the border element.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static IContainer BorderColor(this IContainer element, Color color)
        {
            var border = element as Border ?? new Border();
            border.Color = color;
            return element.Element(border);
        }
    }
}