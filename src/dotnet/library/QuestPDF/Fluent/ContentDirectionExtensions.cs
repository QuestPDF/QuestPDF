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
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="contentDirection.ltr.remarks"]/*' />
        public static IContainer ContentFromLeftToRight(this IContainer element)
        {
            return element.ContentDirection(Infrastructure.ContentDirection.LeftToRight);
        }
        
        /// <summary>
        /// Sets the right-to-left (RTL) direction for its entire content.
        /// <a href="https://www.questpdf.com/api-reference/content-direction.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="contentDirection.rtl.remarks"]/*' />
        public static IContainer ContentFromRightToLeft(this IContainer element)
        {
            return element.ContentDirection(Infrastructure.ContentDirection.RightToLeft);
        }
    }
}