using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class ColumnDescriptor
    {
        internal Column Column { get; } = new();

        /// <summary>
        /// Adjusts spacing between items.
        /// </summary>
        public void Spacing(float value, Unit unit = Unit.Point)
        {
            Column.Spacing = value.ToPoints(unit);
        }
        
        /// <summary>
        /// Adds a new item to the column element.
        /// </summary>
        /// <returns>The container to the newly created item.</returns>
        public IContainer Item()
        {
            var container = new Container();
            
            Column.Items.Add(new ColumnItem
            {
                Child = container
            });
            
            return container;
        }
    }
    
    public static class ColumnExtensions
    {
        [Obsolete("This element has been renamed since version 2022.2. Please use the 'Column' method.")]
        public static void Stack(this IContainer element, Action<ColumnDescriptor> handler)
        {
            element.Column(handler);
        }
        
        /// <summary>
        /// Draws a collection of elements vertically (from top to bottom).
        /// <br />
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