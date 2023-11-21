using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ShrinkExtensions
    {
        private static IContainer Shrink(this IContainer element, bool? vertical = null, bool? horizontal = null)
        {
            var shrink = element as Shrink ?? new Shrink();

            if (vertical.HasValue)
                shrink.Vertical = vertical.Value;

            if (horizontal.HasValue)
                shrink.Horizontal = horizontal.Value;

            return element.Element(shrink);
        }

        /// <summary>
        /// Renders its content in the most compact size achievable. 
        /// Ideal for situations where the parent element provides more space than necessary.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/shrink.html">Learn more</a>
        /// </summary>
        public static IContainer Shrink(this IContainer element)
        {
            return element.Shrink(vertical: true, horizontal: true);
        }

        /// <summary>
        /// Minimizes content height to the minimum, optimizing vertical space.
        /// Ideal for situations where the parent element provides more space than necessary.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/shrink.html">Learn more</a>
        /// </summary>
        public static IContainer ShrinkVertical(this IContainer element)
        {
            return element.Shrink(vertical: true);
        }

        /// <summary>
        /// Minimizes content width to the minimum, optimizing horizontal space.
        /// Ideal for situations where the parent element provides more space than necessary.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/shrink.html">Learn more</a>
        /// </summary>
        public static IContainer ShrinkHorizontal(this IContainer element)
        {
            return element.Shrink(horizontal: true);
        }

        #region Obsolete

        [Obsolete("This element has been renamed since version 2022.1. Please use the Shrink method.")]
        public static IContainer Box(this IContainer element)
        {
            return element.Shrink();
        }
        
        [Obsolete("This element has been renamed since version 2023.11. Please use the Shrink method.")]
        public static IContainer MinimalBox(this IContainer element)
        {
            return element.Shrink();
        }

        #endregion
    }
}