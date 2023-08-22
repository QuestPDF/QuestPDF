using System;
using QuestPDF.Elements;
using QuestPDF.Helpers;
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
        
        /// <summary>
        /// Sets a uniform border (all edges) for its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer Border(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .BorderHorizontal(value, unit)
                .BorderVertical(value, unit);
        }
        
        /// <summary>
        /// Sets a vertical border (left and right) for its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderVertical(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .BorderLeft(value, unit)
                .BorderRight(value, unit);
        }
        
        /// <summary>
        /// Sets a horizontal border (top and bottom) for its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderHorizontal(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .BorderTop(value, unit)
                .BorderBottom(value, unit);
        }
        
        /// <summary>
        /// Sets a border on the left side of its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderLeft(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.Left = value.ToPoints(unit));
        }
        
        /// <summary>
        /// Sets a border on the right side of its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderRight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.Right = value.ToPoints(unit));
        }
        
        /// <summary>
        /// Sets a border on the top side of its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderTop(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.Top = value.ToPoints(unit));
        }
        
        /// <summary>
        /// Sets a border on the bottom side of its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>        
        public static IContainer BorderBottom(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Border(x => x.Bottom = value.ToPoints(unit));
        }
        
        /// <summary>
        /// Adjusts color of the border element.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static IContainer BorderColor(this IContainer element, string color)
        {
            ColorValidator.Validate(color);
            return element.Border(x => x.Color = color);
        }
    }
}