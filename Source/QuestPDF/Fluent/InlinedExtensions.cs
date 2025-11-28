using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public sealed class InlinedDescriptor
    {
        internal Inlined Inlined { get; } = new Inlined();

        internal InlinedDescriptor()
        {
            
        }
        
        #region Spacing
        
        /// <summary>
        /// Sets the vertical and horizontal gaps between items.
        /// </summary>
        public void Spacing(float value, Unit unit = Unit.Point)
        {
            VerticalSpacing(value, unit);
            HorizontalSpacing(value, unit);
        }
        
        /// <summary>
        /// Sets the vertical gaps between items.
        /// </summary>
        public void VerticalSpacing(float value, Unit unit = Unit.Point)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "The Inlined vertical spacing cannot be negative.");
            
            Inlined.VerticalSpacing = value.ToPoints(unit);
        }

        /// <summary>
        /// Sets the horizontal gaps between items.
        /// </summary>
        public void HorizontalSpacing(float value, Unit unit = Unit.Point)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "The Inlined horizontal spacing cannot be negative.");
            
            Inlined.HorizontalSpacing = value.ToPoints(unit);
        }
        
        #endregion
        
        #region Baseline

        /// <summary>
        /// Positions items vertically such that their top edges align on a single line.
        /// </summary>
        public void BaselineTop()
        {
            Inlined.BaselineAlignment = VerticalAlignment.Top;
        }

        /// <summary>
        /// Positions items to have their centers in a straight horizontal line.
        /// </summary>
        public void BaselineMiddle()
        {
            Inlined.BaselineAlignment = VerticalAlignment.Middle;
        }

        /// <summary>
        /// Positions items vertically such that their bottom edges align on a single line.
        /// </summary>
        public void BaselineBottom()
        {
            Inlined.BaselineAlignment = VerticalAlignment.Bottom;
        }

        #endregion

        #region Horizontal Alignment
        
        internal void Alignment(InlinedAlignment? alignment)
        {
            Inlined.ElementsAlignment = alignment;
        }

        /// <summary>
        /// Aligns items horizontally to the left side.
        /// </summary>
        public void AlignLeft()
        {
            Inlined.ElementsAlignment = InlinedAlignment.Left;
        }

        /// <summary>
        /// Aligns items horizontally to the center.
        /// </summary>
        public void AlignCenter()
        {
            Inlined.ElementsAlignment = InlinedAlignment.Center;
        }

        /// <summary>
        /// Aligns items horizontally to the left right.
        /// </summary>
        public void AlignRight()
        {
            Inlined.ElementsAlignment = InlinedAlignment.Right;
        }

        /// <summary>
        /// Distributes items horizontally, ensuring even spacing from edge to edge of the container.
        /// </summary>
        public void AlignJustify()
        {
            Inlined.ElementsAlignment = InlinedAlignment.Justify;
        }
        
        /// <summary>
        /// Spaces items equally in a horizontal arrangement, both between items and at the ends.
        /// </summary>
        public void AlignSpaceAround()
        {
            Inlined.ElementsAlignment = InlinedAlignment.SpaceAround;
        }

        #endregion

        /// <summary>
        /// Adds a new item to the container.
        /// </summary>
        /// <returns>The container of the newly created item.</returns>
        public IContainer Item()
        {
            var container = new Constrained();
            Inlined.Elements.Add(container);
            return container;
        }
    }
    
    public static class InlinedExtensions
    {
        /// <summary>
        /// Arranges its items sequentially in a line, wrapping to the next line if necessary.
        /// <a href="https://www.questpdf.com/api-reference/inlined.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// Supports the paging functionality.
        /// </remarks>
        /// <param name="handler">Handler to configure content of this container, as well as spacing and items alignment.</param>
        public static void Inlined(this IContainer element, Action<InlinedDescriptor> handler)
        {
            var descriptor = new InlinedDescriptor();
            handler(descriptor);
            
            element.Element(descriptor.Inlined);
        }
    }
}