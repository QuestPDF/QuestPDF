using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class RotateExtensions
    {
        private static IContainer SimpleRotate(this IContainer element, int turnDirection)
        {
            var scale = element as SimpleRotate ?? new SimpleRotate();
            scale.TurnCount += turnDirection;
            return element.Element(scale);
        }
        
        /// <summary>
        /// Rotates its content 90 degrees counterclockwise.
        /// <a href="https://www.questpdf.com/api-reference/rotate.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// Note: Rotation can alter certain attributes; for example, 'width' might effectively become 'height'.
        /// </remarks>
        public static IContainer RotateLeft(this IContainer element)
        {
            return element.SimpleRotate(-1);
        }
        
        /// <summary>
        /// Rotates its content 90 degrees clockwise.
        /// <a href="https://www.questpdf.com/api-reference/rotate.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// Note: Rotation can alter certain attributes; for example, 'width' might effectively become 'height'.
        /// </remarks>
        public static IContainer RotateRight(this IContainer element)
        {
            return element.SimpleRotate(1);
        }
        
        /// <summary>
        /// Rotates its content clockwise by a given angle.
        /// <a href="https://www.questpdf.com/api-reference/rotate.html">Learn more</a>
        /// </summary>
        /// <param name="angle">Rotation angle in degrees. A value of 360 degrees represents a full rotation.</param>
        public static IContainer Rotate(this IContainer element, float angle)
        {
            var scale = element as Rotate ?? new Rotate();
            scale.Angle += angle;
            return element.Element(scale);
        }
    }
}