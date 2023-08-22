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
        
        /// <summary>
        /// Rotates its content 90 degrees counterclockwise.
        /// </summary>
        /// <remarks>
        /// Note: Rotation can alter certain attributes; for example, 'width' might effectively become 'height'.
        /// </remarks>
        public static IContainer RotateLeft(this IContainer element)
        {
            return element.SimpleRotate(x => x.TurnCount--);
        }
        
        /// <summary>
        /// Rotates its content 90 degrees clockwise.
        /// </summary>
        /// <remarks>
        /// Note: Rotation can alter certain attributes; for example, 'width' might effectively become 'height'.
        /// </remarks>
        public static IContainer RotateRight(this IContainer element)
        {
            return element.SimpleRotate(x => x.TurnCount++);
        }
        
        /// <summary>
        /// Rotates its content clockwise by a given angle.
        /// </summary>
        /// <param name="angle">Rotation angle in degrees. A value of 360 degrees represents a full rotation.</param>
        public static IContainer Rotate(this IContainer element, float angle)
        {
            return element.Element(new Rotate
            {
                Angle = angle
            });
        }
    }
}