using System;
using QuestPDF.Drawing;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;
using Container = System.ComponentModel.Container;

namespace QuestPDF.Fluent
{
    public class ColumnDescriptor
    {
        internal Column Column { get; set; }

        public void Spacing(float value, Unit unit = Unit.Point)
        {
            Column.Spacing = value.ToPoints(unit);
        }
        
        public IContainer Item()
        {
            var columnItem = ElementCacheManager.Get<ColumnItem>();
            Column.Items.Add(columnItem);
            return columnItem;
        }
    }
    
    public static class ColumnExtensions
    {
        [Obsolete("This element has been renamed since version 2022.2. Please use the 'Column' method.")]
        public static void Stack(this IContainer element, Action<ColumnDescriptor> handler)
        {
            element.Column(handler);
        }
        
        public static void Column(this IContainer element, Action<ColumnDescriptor> handler)
        {
            var descriptor = ElementCacheManager.Get<ColumnDescriptor>();
            descriptor.Column = ElementCacheManager.Get<Column>();
            handler(descriptor);
            element.Element(descriptor.Column);
        }
    }
}