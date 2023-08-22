using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ContentDirectionExtensions
    {
        internal static IContainer ContentDirection(this IContainer element, ContentDirection direction)
        {
            return element.Element(new ContentDirectionSetter
            {
                ContentDirection = direction
            });
        }
        
        /// <summary>
        /// Sets the left-to-right (LTR) direction for its entire content.
        /// <a href="https://www.questpdf.com/api-reference/content-direction.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// The content direction affects various layout structures. In LTR mode, items are typically aligned to the left. 
        /// This mode also influences the direction of items in certain layouts. For instance, in a row element with LTR mode, 
        /// the first item is positioned on the left, while the last item is on the right.
        /// </remarks>
        public static IContainer ContentFromLeftToRight(this IContainer element)
        {
            return element.ContentDirection(Infrastructure.ContentDirection.LeftToRight);
        }
        
        /// <summary>
        /// Sets the left-to-right (LTR) direction for its entire content.
        /// <a href="https://www.questpdf.com/api-reference/content-direction.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// The content direction affects various layout structures. In RTL mode, items are typically aligned to the right. 
        /// This mode also influences the direction of items in certain layouts. For instance, in a row element with RTL mode, 
        /// the first item is positioned on the right, while the last item is on the left.
        /// </remarks>
        public static IContainer ContentFromRightToLeft(this IContainer element)
        {
            return element.ContentDirection(Infrastructure.ContentDirection.RightToLeft);
        }
    }
}