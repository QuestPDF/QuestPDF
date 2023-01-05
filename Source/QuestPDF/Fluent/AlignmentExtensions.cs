using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class AlignmentExtensions
    {
        private static IContainer Alignment(this IContainer element, Action<Alignment> handler)
        {
            var alignment = element as Alignment ?? new Alignment();
            handler(alignment);
            
            return element.Element(alignment);
        }
        
        public static IContainer AlignLeft(this IContainer element)
        {
            return element.Alignment(x => x.Horizontal = HorizontalAlignment.Left);
        }
        
        public static IContainer AlignCenter(this IContainer element)
        {
            return element.Alignment(x => x.Horizontal = HorizontalAlignment.Center);
        }
        
        public static IContainer AlignRight(this IContainer element)
        {
            return element.Alignment(x => x.Horizontal = HorizontalAlignment.Right);
        }
        
        public static IContainer AlignTop(this IContainer element)
        {
            return element.Alignment(x => x.Vertical = VerticalAlignment.Top);
        }
        
        public static IContainer AlignMiddle(this IContainer element)
        {
            return element.Alignment(x => x.Vertical = VerticalAlignment.Middle);
        }
        
        public static IContainer AlignBottom(this IContainer element)
        {
            return element.Alignment(x => x.Vertical = VerticalAlignment.Bottom);
        }
    }
}