using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class RotateExtensions
    {
        private static IContainer SimpleRotate(this IContainer element, Action<SimpleRotate> handler)
        {
            var scale = element as SimpleRotate ?? new SimpleRotate();
            handler(scale);
            
            return element.Element(scale);
        }
        
        public static IContainer RotateLeft(this IContainer element)
        {
            return element.SimpleRotate(x => x.TurnCount--);
        }
        
        public static IContainer RotateRight(this IContainer element)
        {
            return element.SimpleRotate(x => x.TurnCount++);
        }
        
        /// <param name="angle">In degrees</param>
        public static IContainer Rotate(this IContainer element, float angle)
        {
            return element.Element(new Rotate
            {
                Angle = angle
            });
        }
    }
}