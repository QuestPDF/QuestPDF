using System.Linq.Expressions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ContentDirectionExtensions
    {
        internal static IContainer ContentDirection(this IContainer element, ContentDirectionType contentDirectionType)
        {
            var contentDirection = new ContentDirection
            {
                Direction = contentDirectionType
            };

            return element.Element(contentDirection);
        }
        
        public static IContainer ContentDirectionLeftToRight(this IContainer element)
        {
            return element.ContentDirection(ContentDirectionType.LeftToRight);
        }
        
        public static IContainer ContentDirectionRightToLeft(this IContainer element)
        {
            return element.ContentDirection(ContentDirectionType.RightToLeft);
        }
    }
} 