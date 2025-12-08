using System;
using System.Diagnostics.CodeAnalysis;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public sealed class RowDescriptor
    {
        internal Row Row { get; } = new();

        internal RowDescriptor()
        {
            
        }
        
        /// <summary>
        /// Adjusts horizontal spacing between items.
        /// </summary>
        public void Spacing(float spacing, Unit unit = Unit.Point)
        {
            if (spacing < 0)
                throw new ArgumentOutOfRangeException(nameof(spacing), "The row spacing cannot be negative.");
            
            Row.Spacing = spacing.ToPoints(unit);
        }

        private IContainer Item(RowItemType type, float size = 0)
        {
            var item = new RowItem
            {
                Type = type,
                Size = size
            };
            
            Row.Items.Add(item);
            return item;
        }
        
        [Obsolete("This element has been renamed since version 2022.2. Please use the RelativeItem method.")]
        [ExcludeFromCodeCoverage]
        public IContainer RelativeColumn(float size = 1)
        {
            return Item(RowItemType.Relative, size);
        }
        
        [Obsolete("This element has been renamed since version 2022.2. Please use the ConstantItem method.")]
        [ExcludeFromCodeCoverage]
        public IContainer ConstantColumn(float size)
        {
            return Item(RowItemType.Constant, size);
        }

        /// <summary>
        /// Adds a new item to the row element. This item occupies space proportionally to other relative items.
        /// </summary>
        /// <example>
        /// For a row element with a width of 100 points that has three items (a relative item of size 1, a relative item of size 5, and a constant item of size 10 points),
        /// the items will occupy sizes of 15 points, 75 points, and 10 points respectively.
        /// </example>
        /// <returns>The container of the newly added item.</returns>
        public IContainer RelativeItem(float size = 1)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "The relative item size must be greater than zero.");
            
            return Item(RowItemType.Relative, size);
        }
        
        /// <summary>
        /// Adds a new item to the row element with a specified constant size.
        /// </summary>
        /// <returns>The container of the newly created item.</returns>
        public IContainer ConstantItem(float size, Unit unit = Unit.Point)
        {
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size), "The constant item size cannot be negative.");
            
            return Item(RowItemType.Constant, size.ToPoints(unit));
        }

        /// <summary>
        /// Adds a new item to the row element. The size of this item adjusts based on its content.
        /// </summary>
        /// <remarks>
        /// <para>The AutoItem requests as much horizontal space as its content requires.</para>
        /// <para>It doesn't adjust its size based on other items and may frequently result in a <see cref="DocumentLayoutException" />.</para>
        /// <para>It's recommended to use this API in conjunction with the <see cref="ConstrainedExtensions.MaxWidth">MaxWidth</see> element.</para>
        /// </remarks>
        /// <returns>The container of the newly created item.</returns>
        public IContainer AutoItem()
        {
            return Item(RowItemType.Auto);
        }
    }
    
    public static class RowExtensions
    {
        /// <summary>
        /// Draws a collection of elements horizontally.
        /// Depending on the content direction mode, elements will be drawn from left to right, or from right to left.
        /// <br />
        /// <a href="https://www.questpdf.com/api-reference/row.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// <para>Supports paging.</para>
        /// <para>Depending on its content, the Row element may repeatedly draw certain items across multiple pages. Use the <see cref="ElementExtensions.ShowOnce">ShowOnce</see> element to modify this behavior if it's not desired.</para>
        /// </remarks>
        /// <param name="handler">The action to configure the row's content.</param>
        public static void Row(this IContainer element, Action<RowDescriptor> handler)
        {
            var descriptor = new RowDescriptor();
            handler(descriptor);
            element.Element(descriptor.Row);
        }
    }
}