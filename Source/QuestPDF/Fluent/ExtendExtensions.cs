using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ExtendExtensions
    {
        private static IContainer Extend(this IContainer element, bool vertical = false, bool horizontal = false)
        {
            var extend = element as Extend ?? new Extend();

            extend.ExtendVertical |= vertical;
            extend.ExtendHorizontal |= horizontal;
            
            return element.Element(extend);
        }
        
        /// <summary>
        /// Forces its content to occupy entire available space, maximizing both width and height.
        /// <a href="https://www.questpdf.com/api-reference/extend.html">Learn more</a>
        /// </summary>
        public static IContainer Extend(this IContainer element)
        {
            return element.Extend(horizontal: true, vertical: true);
        }
        
        /// <summary>
        /// Forces its content to occupy entire available vertical space, maximizing height usage.
        /// <a href="https://www.questpdf.com/api-reference/extend.html">Learn more</a>
        /// </summary>
        public static IContainer ExtendVertical(this IContainer element)
        {
            return element.Extend(vertical: true);
        }
        
        /// <summary>
        /// Expands its content to occupy entire available horizontal space, maximizing width usage.
        /// <a href="https://www.questpdf.com/api-reference/extend.html">Learn more</a>
        /// </summary>
        public static IContainer ExtendHorizontal(this IContainer element)
        {
            return element.Extend(horizontal: true);
        }
    }
}