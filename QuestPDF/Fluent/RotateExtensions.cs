using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class RotateExtensions
    {
        private static IContainer Rotate(this IContainer element, Action<Rotate> handler)
        {
            var scale = element as Rotate ?? new Rotate();
            handler(scale);
            
            return element.Element(scale);
        }
        
        public static IContainer RotateLeft(this IContainer element)
        {
            return element.Rotate(x => x.TurnCount--);
        }
        
        public static IContainer RotateRight(this IContainer element)
        {
            return element.Rotate(x => x.TurnCount++);
        }
    }
}