using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Layer : ContainerElement
    {
        public bool IsPrimary { get; set; }
    }
    
    internal class Layers : Element, ICacheable
    {
        public List<Layer> Children { get; set; } = new List<Layer>();
        
        internal override IEnumerable<Element?> GetChildren()
        {
            return Children;
        }
        
        internal override void CreateProxy(Func<Element?, Element?> create)
        {
            Children.ForEach(x => x.Child = create(x.Child));
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            return Children
                .Single(x => x.IsPrimary)
                .Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            Children
                .Where(x => x.Measure(availableSpace).Type != SpacePlanType.Wrap)
                .ToList()
                .ForEach(x => x.Draw(availableSpace));
        }
    }
}