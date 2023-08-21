using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ConstrainedExtensions
    {
        private static IContainer Constrained(this IContainer element, Action<Constrained> handler)
        {
            var constrained = element as Constrained ?? new Constrained();
            handler(constrained);
            
            return element.Element(constrained);
        }
        
        /// <summary>
        /// Sets the exact width of its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/width.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified exact width.</returns>
        public static IContainer Width(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .MinWidth(value, unit)
                .MaxWidth(value, unit);
        }
        
        /// <summary>
        /// Sets the minimum width of its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/width.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified minimum width.</returns>
        public static IContainer MinWidth(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Constrained(x => x.MinWidth = value.ToPoints(unit));
        }
        
        /// <summary>
        /// Sets the maximum width of its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/width.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified maximum width.</returns>
        public static IContainer MaxWidth(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Constrained(x => x.MaxWidth = value.ToPoints(unit));
        }
        
        /// <summary>
        /// Sets the exact height of its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/height.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified exact height.</returns>
        public static IContainer Height(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element
                .MinHeight(value, unit)
                .MaxHeight(value, unit);
        }
        
        /// <summary>
        /// Sets the minimum height of its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/height.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified minimum height.</returns>
        public static IContainer MinHeight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Constrained(x => x.MinHeight = value.ToPoints(unit));
        }
        
        /// <summary>
        /// Sets the maximum height of its content.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/height.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified maximum height.</returns>
        public static IContainer MaxHeight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.Constrained(x => x.MaxHeight = value.ToPoints(unit));
        }
    }
}