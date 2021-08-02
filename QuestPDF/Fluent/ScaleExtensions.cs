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
            return element.ScaleHorizontal(value).ScaleVertical(value);
        }
        
        public static IContainer ScaleHorizontal(this IContainer element, float value)
        {
            return element.Scale(x => x.ScaleX = value);
        }
        
        public static IContainer FlipHorizontal(this IContainer element)
        {
            return element.ScaleHorizontal(-1);
        }
        
        public static IContainer ScaleVertical(this IContainer element, float value)
        {
            return element.Scale(x => x.ScaleY = value);
        }
        
        public static IContainer FlipVertical(this IContainer element)
        {
            return element.ScaleVertical(-1);
        }
        
        public static IContainer FlipOver(this IContainer element)
        {
            return element.FlipHorizontal().FlipVertical();
        }
    }
}