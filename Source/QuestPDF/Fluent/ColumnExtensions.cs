using System;
using System.Diagnostics.CodeAnalysis;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public sealed class ColumnDescriptor
    {
        internal Column Column { get; } = new();

        internal ColumnDescriptor()
        {
            
        }

        /// <summary>
        /// Adjusts vertical spacing between items.
        /// </summary>
        public void Spacing(float value, Unit unit = Unit.Point)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "The column spacing cannot be negative.");
            
            Column.Spacing = value.ToPoints(unit);
        }
        
        /// <summary>
        /// Adds a new item to the column element.
        /// </summary>
        /// <returns>The container of the newly created item.</returns>
        public IContainer Item()
        {
            var container = new Container();
            Column.Items.Add(container);
            return container;
        }
    }
    
    public static class ColumnExtensions
    {
        [Obsolete("This element has been renamed since version 2022.2. Please use the 'Column' method.")]
        [ExcludeFromCodeCoverage]
        public static void Stack(this IContainer element, Action<ColumnDescriptor> handler)
        {
            element.Column(handler);
        }
        
        /// <summary>
        /// Draws a collection of elements vertically (from top to bottom).
        /// <a href="https://www.questpdf.com/api-reference/column.html">Learn more</a>
        /// </summary>
        /// <remarks>
        /// Supports paging.
        /// </remarks>
        /// <param name="handler">The action to configure the column's content.</param>
        public static void Column(this IContainer element, Action<ColumnDescriptor> handler)
        {
            var descriptor = new ColumnDescriptor();
            handler(descriptor);
            element.Element(descriptor.Column);
        }
    }
}