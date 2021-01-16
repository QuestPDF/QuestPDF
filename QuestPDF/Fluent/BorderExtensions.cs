using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class BorderExtensions
    {
        private static IContainer Border(this IContainer element, Action<Border> handler)
        {
            var border = element as Border ?? new Border();
            handler(border);
            
            return element.Element(border);
        }
        
        public static IContainer Border(this IContainer element, float value)
        {
            return element.BorderHorizontal(value).BorderVertical(value);
        }
        
        public static IContainer BorderVertical(this IContainer element, float value)
        {
            return element.BorderLeft(value).BorderRight(value);
        }
        
        public static IContainer BorderHorizontal(this IContainer element, float value)
        {
            return element.BorderTop(value).BorderBottom(value);
        }
        
        public static IContainer BorderLeft(this IContainer element, float value)
        {
            return element.Border(x => x.Left = value);
        }
        
        public static IContainer BorderRight(this IContainer element, float value)
        {
            return element.Border(x => x.Right = value);
        }
        
        public static IContainer BorderTop(this IContainer element, float value)
        {
            return element.Border(x => x.Top = value);
        }
        
        public static IContainer BorderBottom(this IContainer element, float value)
        {
            return element.Border(x => x.Bottom = value);
        }
        
        public static IContainer BorderColor(this IContainer element, string color)
        {
            return element.Border(x => x.Color = color);
        }
    }
}