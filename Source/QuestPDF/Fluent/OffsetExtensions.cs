using System;
using System.Diagnostics.CodeAnalysis;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class OffsetExtensions
    {
        private static IContainer Offset(this IContainer element, float x = 0, float y = 0)
        {
            var offset = element as Offset ?? new Offset();

            offset.OffsetX += x;
            offset.OffsetY += y;

            return element.Element(offset);
        }

        /// <summary>
        /// Moves content along the horizontal axis.
        /// A positive value moves content to the right; a negative value moves it to the left.
        /// Does not alter the available space.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/offset.html">Learn more</a>
        /// </summary>
        public static IContainer OffsetX(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Offset(x: value);
        }
   
        /// <summary>
        /// Moves content along the vertical axis.
        /// A positive value moves content downwards, a negative value moves it upwards.
        /// Does not alter the available space.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/offset.html">Learn more</a>
        /// </summary>
        public static IContainer OffsetY(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Offset(y: value);
        }
    }
    
    #region Obsolete
    
    [Obsolete("This class has been replaced by OffsetExtensions.")]
    public static class TranslateExtensions
    {
        [Obsolete("This element has been renamed since version 2026.6. Please use the OffsetX method.")]
        [ExcludeFromCodeCoverage]
        public static IContainer TranslateX(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.OffsetX(value, unit);
        }

        [Obsolete("This element has been renamed since version 2026.6. Please use the OffsetY method.")]
        [ExcludeFromCodeCoverage]
        public static IContainer TranslateY(this IContainer element, float value, Unit unit = Unit.Point)
        {
            return element.OffsetY(value, unit);
        }
    }
    
    #endregion
}