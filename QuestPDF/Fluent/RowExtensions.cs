using System;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class RowDescriptor
    {
        private Row Row { get; }

        internal RowDescriptor(Row row)
        {
            Row = row;
        }
        
        public IContainer ConstantColumn(float width)
        {
            var element = new ConstantRowElement()
            {
                Width = width
            };
            
            Row.Children.Add(element);
            return element;
        }
        
        public IContainer RelativeColumn(float width = 1)
        {
            var element = new RelativeRowElement()
            {
                Width = width
            };
            
            Row.Children.Add(element);
            return element;
        }
    }
    
    public static class RowExtensions
    {
        public static void Row(this IContainer element, Action<RowDescriptor> handler)
        {
            var row = new Row();
            element.Element(row);
            
            var descriptor = new RowDescriptor(row);
            handler(descriptor);
        }
    }
}