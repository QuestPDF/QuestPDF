using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Elements;
using QuestPDF.Infrastructure;

namespace QuestPDF.Fluent
{
    public class RowDescriptor
    {
        private List<RowElement> Elements { get; } = new List<RowElement>();
        private float RowSpacing { get; set; } = 0;
        
        public void Spacing(float value)
        {
            RowSpacing = value;
        }
        
        public IContainer ConstantColumn(float width)
        {
            var element = new ConstantRowElement(width);
            
            Elements.Add(element);
            return element;
        }
        
        public IContainer RelativeColumn(float width = 1)
        {
            var element = new RelativeRowElement(width);
            
            Elements.Add(element);
            return element;
        }
        
        internal Element CreateRow()
        {
            return new TreeRow
            {
                Children = Elements,
                Spacing = RowSpacing
            };
        }
        
        internal Element CreateRow2()
        {
            if (Elements.Count == 0)
                return Empty.Instance;
            
            if (RowSpacing <= Size.Epsilon)
                return new Row
                {
                    Children = Elements
                };

            var children = Elements
                .SelectMany(x => new[] {x, new ConstantRowElement(RowSpacing) })
                .ToList();

            var row = new Row
            {
                Children = children
            };
            
            return new Padding
            {
                Right = -RowSpacing,
                Child = row
            };
        }
    }
    
    public static class RowExtensions
    {
        public static void Row(this IContainer element, Action<RowDescriptor> handler)
        {
            var descriptor = new RowDescriptor();
            handler(descriptor);
            element.Element(descriptor.CreateRow());
            //element.Element(descriptor.CreateRow2());
        }
    }
}