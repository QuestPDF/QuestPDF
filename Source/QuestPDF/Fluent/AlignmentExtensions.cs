using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class AlignmentExtensions
    {
        #region Horizontal
        
        private static IContainer AlignHorizontal(this IContainer element, HorizontalAlignment horizontalAlignment)
        {
            var alignment = element as Alignment ?? new Alignment();
            alignment.Horizontal = horizontalAlignment;
            return element.Element(alignment);
        }
        
        /// <summary>
        /// Aligns content horizontally to the left side.
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignLeft(this IContainer element)
        {
            return element.AlignHorizontal(HorizontalAlignment.Left);
        }
        
        /// <summary>
        /// Aligns content horizontally to the center, ensuring equal space on both left and right sides.
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignCenter(this IContainer element)
        {
            return element.AlignHorizontal(HorizontalAlignment.Center);
        }
        
        /// <summary>
        /// Aligns its content horizontally to the right side.
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignRight(this IContainer element)
        {
            return element.AlignHorizontal(HorizontalAlignment.Right);
        }
        
        #endregion
        
        #region Vertical
        
        private static IContainer AlignVertical(this IContainer element, VerticalAlignment verticalAlignment)
        {
            var alignment = element as Alignment ?? new Alignment();
            alignment.Vertical = verticalAlignment;
            return element.Element(alignment);
        }
        
        /// <summary>
        /// Aligns content vertically to the upper side.
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignTop(this IContainer element)
        {
            return element.AlignVertical(VerticalAlignment.Top);
        }
        
        /// <summary>
        /// Aligns content vertically to the center, ensuring equal space above and below.
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignMiddle(this IContainer element)
        {
            return element.AlignVertical(VerticalAlignment.Middle);
        }
        
        /// <summary>
        /// Aligns content vertically to the bottom side.
        /// <a href="https://www.questpdf.com/api-reference/alignment.html">Learn more</a>
        /// </summary>
        public static IContainer AlignBottom(this IContainer element)
        {
            return element.AlignVertical(VerticalAlignment.Bottom);
        }
        
        #endregion
    }
}