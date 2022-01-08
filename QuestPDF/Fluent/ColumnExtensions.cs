using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class ColumnDescriptor
    {
        //internal column Column { get; } = new();
        internal Column Column { get; } = new();

        public void Spacing(float value)
        {
            Column.Spacing = value;
        }
        
        public IContainer Item()
        {
            var container = new Container();
            
            Column.Items.Add(new ColumnItem()
            {
                Child = container
            });
            
            return container;
        }
    }
    
    public static class ColumnExtensions
    {
        // TODO: deprecated Box method in QuestPDF 2022.2
        [Obsolete("This element has been renamed. Please use the Column method.")]
        public static void Stack(this IContainer element, Action<ColumnDescriptor> handler)
        {
            element.Column(handler);
        }
        
        public static void Column(this IContainer element, Action<ColumnDescriptor> handler)
        {
            var descriptor = new ColumnDescriptor();
            handler(descriptor);
            element.Element(descriptor.Column);
            //element.Component(descriptor.Column);
        }
    }
}