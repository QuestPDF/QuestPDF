using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ExtendExtensions
    {
        private static IContainer Extend(this IContainer element, Action<Extend> handler)
        {
            var extend = element as Extend ?? new Extend();
            handler(extend);
            
            return element.Element(extend);
        }
        
        /// <summary>
        /// Forces its content to occupy entire available space, maximizing both width and height.
        /// <a href="https://www.questpdf.com/api-reference/extend.html">Learn more</a>
        /// </summary>
        public static IContainer Extend(this IContainer element)
        {
            return element.ExtendVertical().ExtendHorizontal();
        }
        
        /// <summary>
        /// Forces its content to occupy entire available vertical space, maximizing height usage.
        /// <a href="https://www.questpdf.com/api-reference/extend.html">Learn more</a>
        /// </summary>
        public static IContainer ExtendVertical(this IContainer element)
        {
            return element.Extend(x => x.ExtendVertical = true);
        }
        
        /// <summary>
        /// Expands its content to occupy entire available horizontal space, maximizing width usage.
        /// <a href="https://www.questpdf.com/api-reference/extend.html">Learn more</a>
        /// </summary>
        public static IContainer ExtendHorizontal(this IContainer element)
        {
            return element.Extend(x => x.ExtendHorizontal = true);
        }
    }
}