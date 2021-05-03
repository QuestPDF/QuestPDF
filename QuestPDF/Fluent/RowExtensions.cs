using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class RowDescriptor
    {
        internal Row Row { get; } = new Row();

        public void Spacing(float value)
        {
            Row.Spacing = value;
        }
        
        public IContainer ConstantColumn(float width)
        {
            var element = new ConstantRowElement(width);
            
            Row.Children.Add(element);
            return element;
        }
        
        public IContainer RelativeColumn(float width = 1)
        {
            var element = new RelativeRowElement(width);
            
            Row.Children.Add(element);
            return element;
        }
    }
    
    public static class RowExtensions
    {
        public static void Row(this IContainer element, Action<RowDescriptor> handler)
        {
            var descriptor = new RowDescriptor();
            handler(descriptor);
            element.Element(descriptor.Row);
        }
    }
}