using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class PaddingExtensions
    { 
        private static IContainer Padding(this IContainer element, Action<Padding> handler)
        {
            var padding = element as Padding ?? new Padding();
            handler(padding);
            
            return element.Element(padding);
        }
        
        public static IContainer Padding(this IContainer element, float value)
        {
            return element.PaddingVertical(value).PaddingHorizontal(value);
        }
        
        public static IContainer PaddingHorizontal(this IContainer element, float value)
        {
            return element.PaddingLeft(value).PaddingRight(value);
        }
        
        public static IContainer PaddingVertical(this IContainer element, float value)
        {
            return element.PaddingTop(value).PaddingBottom(value);
        }
        
        public static IContainer PaddingTop(this IContainer element, float value)
        {
            return element.Padding(x => x.Top += value);
        }
        
        public static IContainer PaddingBottom(this IContainer element, float value)
        {
            return element.Padding(x => x.Bottom += value);
        }
        
        public static IContainer PaddingLeft(this IContainer element, float value)
        {
            return element.Padding(x => x.Left += value);
        }
        
        public static IContainer PaddingRight(this IContainer element, float value)
        {
            return element.Padding(x => x.Right += value);
        }
    }
}