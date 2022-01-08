using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class TranslateExtensions
    {
        private static IContainer Translate(this IContainer element, Action<Translate> handler)
        {
            var translate = element as Translate ?? new Translate();
            handler(translate);
            
            return element.Element(translate);
        }

        public static IContainer TranslateX(this IContainer element, float value)
        {
            return element.Translate(x => x.TranslateX += value);
        }
        
        public static IContainer TranslateY(this IContainer element, float value)
        {
            return element.Translate(x => x.TranslateY += value);
        }
    }
}