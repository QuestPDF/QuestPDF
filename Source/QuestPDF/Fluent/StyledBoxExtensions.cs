using System;
using QuestPDF.Elements;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public static class StyledBoxExtensions
    {
        /// <summary>
        /// Sets a uniform border around the container with the specified thickness and color.
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static IContainer Border(this IContainer element, float all, Color color)
        {
            return element.Border(left: all, top: all, right: all, bottom: all).BorderColor(color);
        }
        
        /// <summary>
        /// Sets a solid background color behind its content.
        /// <a href="https://www.questpdf.com/api-reference/background.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static IContainer Background(this IContainer element, Color color)
        {
            var styledBox = element as StyledBox ?? new StyledBox();
            styledBox.BackgroundColor = color;
            
            return element.Element(styledBox);
        }

        /// <summary>
        /// Applies a linear gradient background to the container with the specified angle and colors.
        /// </summary>
        /// <param name="angle">The angle in degrees of the gradient direction.</param>
        /// <param name="colors">An array of <see cref="Color"/> representing the gradient colors.</param>
        public static IContainer BackgroundLinearGradient(this IContainer element, float angle, Color[] colors)
        {
            var border = element as StyledBox ?? new StyledBox();
            border.BackgroundGradientAngle = angle;
            border.BackgroundGradientColors = colors;
            return element.Element(border);
        }
        
        #region Thickness
        
        private static IContainer Border(this IContainer element, float? top = null, float? bottom = null, float? left = null, float? right = null)
        {
            var styledBox = element as StyledBox ?? new StyledBox();

            if (top < 0)
                throw new ArgumentOutOfRangeException(nameof(top), "The top border cannot be negative.");
            
            if (bottom < 0)
                throw new ArgumentOutOfRangeException(nameof(bottom), "The bottom border cannot be negative.");
            
            if (left < 0)
                throw new ArgumentOutOfRangeException(nameof(left), "The left border cannot be negative.");
            
            if (right < 0)
                throw new ArgumentOutOfRangeException(nameof(right), "The right border cannot be negative.");

            if (styledBox.BorderColor == Colors.Transparent.Hex)
                styledBox.BorderColor = Colors.Black;
            
            if (top.HasValue)
                styledBox.BorderTop = top.Value;
            
            if (bottom.HasValue)
                styledBox.BorderBottom = bottom.Value;
            
            if (left.HasValue)
                styledBox.BorderLeft = left.Value;
            
            if (right.HasValue)
                styledBox.BorderRight = right.Value;

            return element.Element(styledBox);
        }
        
        /// <summary>
        /// Sets a uniform border (all edges) for its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer Border(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(top: value, bottom: value, left: value, right: value);
        }
        
        /// <summary>
        /// Sets a vertical border (left and right) for its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderVertical(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(left: value, right: value);
        }
        
        /// <summary>
        /// Sets a horizontal border (top and bottom) for its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderHorizontal(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(top: value, bottom: value);
        }
        
        /// <summary>
        /// Sets a border on the left side of its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderLeft(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(left: value);
        }
        
        /// <summary>
        /// Sets a border on the right side of its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderRight(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(right: value);
        }
        
        /// <summary>
        /// Sets a border on the top side of its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        public static IContainer BorderTop(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(top: value);
        }
        
        /// <summary>
        /// Sets a border on the bottom side of its content.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>        
        public static IContainer BorderBottom(this IContainer element, float value, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.Border(bottom: value);
        }
        
        #endregion
        
        #region Corner Radius
        
        private static IContainer CornerRadius(this IContainer element, float? topLeft = null, float? topRight = null, float? bottomRight = null, float? bottomLeft = null)
        {
            var styledBox = element as StyledBox ?? new StyledBox();
            
            if (topLeft < 0)
                throw new ArgumentOutOfRangeException(nameof(topLeft), "The top-left border radius cannot be negative.");
            
            if (topRight < 0)
                throw new ArgumentOutOfRangeException(nameof(topRight), "The top-right border radius cannot be negative.");
            
            if (bottomRight < 0)
                throw new ArgumentOutOfRangeException(nameof(bottomRight), "The bottom-right border radius cannot be negative.");
            
            if (bottomLeft < 0)
                throw new ArgumentOutOfRangeException(nameof(bottomLeft), "The bottom-left border radius cannot be negative.");
            
            if (topLeft.HasValue)
                styledBox.BorderRadiusTopLeft = topLeft.Value;
            
            if (topRight.HasValue)
                styledBox.BorderRadiusTopRight = topRight.Value;
            
            if (bottomRight.HasValue)
                styledBox.BorderRadiusBottomRight = bottomRight.Value;
            
            if (bottomLeft.HasValue)
                styledBox.BorderRadiusBottomLeft = bottomLeft.Value;
            
            return element.Element(styledBox);
        }

        /// <summary>
        /// Applies a uniform corner radius to all corners of the container with the specified value and unit.
        /// </summary>
        public static IContainer CornerRadius(this IContainer element, float value = 0, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.CornerRadius(topLeft: value, topRight: value, bottomRight: value, bottomLeft: value);
        }

        /// <summary>
        /// Applies a corner radius to the top-left corner of the container with the specified value and unit.
        /// </summary>
        public static IContainer CornerRadiusTopLeft(this IContainer element, float value = 0, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.CornerRadius(topLeft: value);
        }
        
        /// <summary>
        /// Applies a corner radius to the top-right corner of the container with the specified value and unit.
        /// </summary>
        public static IContainer CornerRadiusTopRight(this IContainer element, float value = 0, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.CornerRadius(topRight: value);
        }
        
        /// <summary>
        /// Applies a border radius to the bottom-left corner of the container with the specified value and unit.
        /// </summary>
        public static IContainer CornerRadiusBottomLeft(this IContainer element, float value = 0, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.CornerRadius(bottomLeft: value);
        }
        
        /// <summary>
        /// Applies a corner radius to the bottom-right corner of the container with the specified value and unit.
        /// </summary>
        public static IContainer CornerRadiusBottomRight(this IContainer element, float value = 0, Unit unit = Unit.Point)
        {
            value = value.ToPoints(unit);
            return element.CornerRadius(bottomRight: value);
        }
        
        #endregion
        
        #region Style
        
        /// <summary>
        /// Adjusts color of the border element.
        /// <a href="https://www.questpdf.com/api-reference/border.html">Learn more</a>
        /// </summary>
        /// <include file='../Resources/Documentation.xml' path='documentation/doc[@for="colorParam"]/*' />
        public static IContainer BorderColor(this IContainer element, Color color)
        {
            var border = element as StyledBox ?? new StyledBox();
            border.BorderColor = color;
            return element.Element(border);
        }

        /// <summary>
        /// Sets a linear gradient for the border with the specified angle and colors.
        /// </summary>
        /// <param name="angle">The angle of the gradient in degrees.</param>
        /// <param name="colors">An array of <see cref="Color"/> objects representing the gradient colors.</param>
        public static IContainer BorderLinearGradient(this IContainer element, float angle, Color[] colors)
        {
            var border = element as StyledBox ?? new StyledBox();
            border.BorderGradientAngle = angle;
            border.BorderGradientColors = colors;
            return element.Element(border);
        }
        
        #endregion

        #region Alignment

        private static IContainer BorderAlignment(this IContainer element, float value)
        {
            var border = element as StyledBox ?? new StyledBox();
            border.BorderAlignment = value;
            return element.Element(border);
        }

        /// <summary>
        /// Aligns the container's border to the outer edge of the element.
        /// </summary>
        public static IContainer BorderAlignmentOutside(this IContainer element)
        {
            return element.BorderAlignment(1);
        }

        /// <summary>
        /// Aligns the border in the middle of the specified container boundaries.
        /// This option is used by default when no alignment is specified.
        /// </summary>
        public static IContainer BorderAlignmentMiddle(this IContainer element)
        {
            return element.BorderAlignment(0.5f);
        }

        /// <summary>
        /// Aligns the border to the inside of the container.
        /// </summary>
        public static IContainer BorderAlignmentInside(this IContainer element)
        {
            return element.BorderAlignment(0);
        }

        #endregion
        
        #region Shadow

        /// <summary>
        /// Applies a shadow to the container using the specified shadow style.
        /// Shadows can enhance the visual depth and separation of elements in a document.
        /// </summary>
        public static IContainer Shadow(this IContainer element, BoxShadowStyle style)
        {
            var styledBox = element as StyledBox ?? new StyledBox();
            styledBox.Shadow = style;
            return element.Element(styledBox);
        }

        #endregion
    }
}