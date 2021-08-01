using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Layer : ContainerElement
    {
        public bool IsPrimary { get; set; }
    }
    
    internal class Layers : Element
    {
        public List<Layer> Children { get; set; } = new List<Layer>();
        
        internal override void HandleVisitor(Action<Element?> visit)
        {
            Children.ForEach(x => x.HandleVisitor(visit));
            base.HandleVisitor(visit);
        }

        internal override ISpacePlan Measure(Size availableSpace)
        {
            return Children
                .Single(x => x.IsPrimary)
                .Measure(availableSpace);
        }

        internal override void Draw(Size availableSpace)
        {
            Children
                .Where(x => x.Measure(availableSpace) is Size)
                .ToList()
                .ForEach(x => x.Draw(availableSpace));
        }
    }
}