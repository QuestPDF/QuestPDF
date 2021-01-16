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
        
        public static IContainer Extend(this IContainer element)
        {
            return element.ExtendVertical().ExtendHorizontal();
        }
        
        public static IContainer ExtendVertical(this IContainer element)
        {
            return element.Extend(x => x.ExtendVertical = true);
        }
        
        public static IContainer ExtendHorizontal(this IContainer element)
        {
            return element.Extend(x => x.ExtendHorizontal = true);
        }
    }
}