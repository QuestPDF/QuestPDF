using System;
using System.Diagnostics.CodeAnalysis;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class RotateExtensions
    {
        private static IContainer RotateLayout(this IContainer element, int turnDirection)
        {
            var rotateLayout = element as RotateLayout ?? new RotateLayout();
            rotateLayout.TurnCount += turnDirection;
            return element.Element(rotateLayout);
        }

        /// <summary>
        /// Rotates its content by 90 degrees counterclockwise, swapping the element's width and height in the layout.
        /// <a href="https://www.questpdf.com/api-reference/rotate.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// Consecutive calls accumulate. Useful for vertical labels, e.g. in narrow table header cells.
        /// </remarks>
        public static IContainer RotateLayoutCounterclockwise(this IContainer element)
        {
            return element.RotateLayout(-1);
        }

        /// <summary>
        /// Rotates its content by 90 degrees clockwise, swapping the element's width and height in the layout.
        /// <a href="https://www.questpdf.com/api-reference/rotate.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// Consecutive calls accumulate. Useful for vertical labels, e.g. in narrow table header cells.
        /// </remarks>
        public static IContainer RotateLayoutClockwise(this IContainer element)
        {
            return element.RotateLayout(1);
        }

        /// <summary>
        /// Rotates its content by an arbitrary angle around its center.
        /// <a href="https://www.questpdf.com/api-reference/rotate.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// Does not affect the layout: the element occupies the same space as its non-rotated content, and the rotated content may overflow it.
        /// For layout-aware rotation, use <see cref="RotateLayoutClockwise" /> or <see cref="RotateLayoutCounterclockwise" />. Consecutive calls accumulate.
        /// </remarks>
        /// <param name="angle">Rotation angle in degrees. Positive values rotate clockwise, negative values counterclockwise.</param>
        public static IContainer Rotate(this IContainer element, float angle)
        {
            var rotate = element as Rotate ?? new Rotate();
            rotate.Angle += angle;
            return element.Element(rotate);
        }

        #region Obsolete

        [Obsolete("This element has been renamed since version 2026.8. Please use the RotateLayoutCounterclockwise method.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [ExcludeFromCodeCoverage]
        public static IContainer RotateLeft(this IContainer element)
        {
            return element.RotateLayoutCounterclockwise();
        }

        [Obsolete("This element has been renamed since version 2026.8. Please use the RotateLayoutClockwise method.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [ExcludeFromCodeCoverage]
        public static IContainer RotateRight(this IContainer element)
        {
            return element.RotateLayoutClockwise();
        }

        #endregion
    }
}
