using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ConstrainedExtensions
    {
        #region Width
        
        private static IContainer ConstrainedWidth(this IContainer element, float? min = null, float? max = null)
        {
            var constrained = element as Constrained ?? new Constrained();

            if (min.HasValue)
                constrained.MinWidth = min;
            
            if (max.HasValue)
                constrained.MaxWidth = max;
            
            return element.Element(constrained);
        }
        
        /// <summary>
        /// Sets the exact width of its content.
        /// <a href="https://www.questpdf.com/api-reference/width.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified exact width.</returns>
        public static IContainer Width(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.ConstrainedWidth(min: value, max: value);
        }
        
        /// <summary>
        /// Sets the minimum width of its content.
        /// <a href="https://www.questpdf.com/api-reference/width.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified minimum width.</returns>
        public static IContainer MinWidth(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.ConstrainedWidth(min: value);
        }
        
        /// <summary>
        /// Sets the maximum width of its content.
        /// <a href="https://www.questpdf.com/api-reference/width.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified maximum width.</returns>
        public static IContainer MaxWidth(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.ConstrainedWidth(max: value);
        }
        
        #endregion
        
        #region Height
        
        private static IContainer ConstrainedHeight(this IContainer element, float? min = null, float? max = null)
        {
            var constrained = element as Constrained ?? new Constrained();

            if (min.HasValue) 
                constrained.MinHeight = min;
            
            if (max.HasValue)
                constrained.MaxHeight = max;
            
            return element.Element(constrained);
        }
        
        /// <summary>
        /// Sets the exact height of its content.
        /// <a href="https://www.questpdf.com/api-reference/height.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified exact height.</returns>
        public static IContainer Height(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.ConstrainedHeight(min: value, max: value);
        }
        
        /// <summary>
        /// Sets the minimum height of its content.
        /// <a href="https://www.questpdf.com/api-reference/height.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified minimum height.</returns>
        public static IContainer MinHeight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.ConstrainedHeight(min: value);
        }
        
        /// <summary>
        /// Sets the maximum height of its content.
        /// <a href="https://www.questpdf.com/api-reference/height.html">Learn more</a>
        /// </summary>
        /// <returns>The container with the specified maximum height.</returns>
        public static IContainer MaxHeight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.ConstrainedHeight(max: value);
        }
        
        #endregion
        
        internal static IContainer EnforceSizeWhenEmpty(this IContainer element)
        {
            (element as Constrained).EnforceSizeWhenEmpty = true;
            return element;
        }
    }
}