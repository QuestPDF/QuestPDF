using System;
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
            return Column(constantWidth: width);
        }
        
        public IContainer RelativeColumn(float width = 1)
        {
            return Column(relativeWidth: width);
        }
        
        private IContainer Column(float constantWidth = 0, float relativeWidth = 0)
        {
            var element = new RowElement(constantWidth, relativeWidth);
            
            Row.Items.Add(element);
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