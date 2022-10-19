using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ShrinkExtensions
    {
        private static IContainer Shrink(this IContainer element, Action<Shrink> handler)
        {
            var shrink = element as Shrink ?? new Shrink();
            handler(shrink);
            
            return element.Element(shrink);
        }
        
        public static IContainer Shrink(this IContainer element)
        {
            return element.ExtendVertical().ExtendHorizontal();
        }
        
        public static IContainer ShrinkVertical(this IContainer element)
        {
            return element.Shrink(x => x.ShrinkVertical = true);
        }
        
        public static IContainer ShrinkHorizontal(this IContainer element)
        {
            return element.Shrink(x => x.ShrinkHorizontal = true);
        }

        #region Obsolete
        
        [Obsolete("This element has been renamed since version 2022.1. Please use the Shrink method.")]
        public static IContainer Box(this IContainer element)
        {
            return element.Element(new Shrink());
        }
        
        [Obsolete("This element has been renamed since version 2022.11. Please use the Shrink method.")]
        public static IContainer MinimalBox(this IContainer element)
        {
            return element.Element(new Shrink());
        }

        #endregion
    }
}