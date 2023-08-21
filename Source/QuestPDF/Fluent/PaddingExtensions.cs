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
        
        /// <summary>
        /// For positive values, adds empty space around its content.
        /// For negative values, pushes its content beyond the edges, increasing available space, similarly to negative HTML margins.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/padding.html">Learn more</a>
        /// </summary>
        public static IContainer Padding(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .PaddingVertical(value, unit)
                .PaddingHorizontal(value, unit);
        }
        
        /// <summary>
        /// For positive values, adds empty space horizontally (left and right) around its content.
        /// For negative values, pushes its content beyond the horizontal edges, increasing available space, similarly to negative HTML margins.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/padding.html">Learn more</a>
        /// </summary>
        public static IContainer PaddingHorizontal(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .PaddingLeft(value, unit)
                .PaddingRight(value, unit);
        }

        /// <summary>
        /// For positive values, adds empty space vertically (top and bottom) around its content.
        /// For negative values, pushes its content beyond the vertical edges, increasing available space, similarly to negative HTML margins.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/padding.html">Learn more</a>
        /// </summary>
        public static IContainer PaddingVertical(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .PaddingTop(value, unit)
                .PaddingBottom(value, unit);
        }
        
        /// <summary>
        /// For positive values, adds empty space above its content.
        /// For negative values, pushes its content beyond the top edge, increasing available space, similarly to negative HTML margins.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/padding.html">Learn more</a>
        /// </summary>
        public static IContainer PaddingTop(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Padding(x => x.Top += value.ToPoints(unit));
        }
        
        /// <summary>
        /// For positive values, adds empty space below its content.
        /// For negative values, pushes its content beyond the bottom edge, increasing available space, similarly to negative HTML margins.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/padding.html">Learn more</a>
        /// </summary>
        public static IContainer PaddingBottom(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Padding(x => x.Bottom += value.ToPoints(unit));
        }
        
        /// <summary>
        /// For positive values, adds empty space to the left of its content.
        /// For negative values, pushes its content beyond the left edge, increasing available space, similarly to negative HTML margins.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/padding.html">Learn more</a>
        /// </summary>
        public static IContainer PaddingLeft(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Padding(x => x.Left += value.ToPoints(unit));
        }
        
        /// <summary>
        /// For positive values, adds empty space to the right of its content.
        /// For negative values, pushes its content beyond the right edge, increasing available space, similarly to negative HTML margins.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/padding.html">Learn more</a>
        /// </summary>
        public static IContainer PaddingRight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Padding(x => x.Right += value.ToPoints(unit));
        }
    }
}