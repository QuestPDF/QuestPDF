using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class ScaleExtensions
    {
        private static IContainer Scale(this IContainer element, Action<Scale> handler)
        {
            var scale = element as Scale ?? new Scale();
            handler(scale);
            
            return element.Element(scale);
        }
        
        public static IContainer Scale(this IContainer element, float value)
        {
            return element.ScaleX(value).ScaleY(value);
        }
        
        public static IContainer ScaleX(this IContainer element, float value)
        {
            return element.Scale(x => x.ScaleX = value);
        }
        
        public static IContainer FlipX(this IContainer element)
        {
            return element.ScaleX(-1);
        }
        
        public static IContainer ScaleY(this IContainer element, float value)
        {
            return element.Scale(x => x.ScaleY = value);
        }
        
        public static IContainer FlipY(this IContainer element)
        {
            return element.ScaleY(-1);
        }
        
        public static IContainer FlipOver(this IContainer element)
        {
            return element.FlipX().FlipY();
        }
    }
}