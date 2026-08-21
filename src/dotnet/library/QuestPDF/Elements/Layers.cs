using System.Collections.Generic;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class Layer : ContainerElement
    {
        public bool IsPrimary { get; set; }
    }

    internal sealed class Layers : Element
    {
        public List<Layer> Children { get; set; } = new();
        private Layer? PrimaryLayer { get; set; }
        
        internal override IReadOnlyList<Element?> GetChildren()
        {
            return Children;
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            PrimaryLayer ??= Children.Single(x => x.IsPrimary);
            
            var measurement = PrimaryLayer.Measure(availableSpace);

            if (measurement.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap("The content of the primary layer does not fit (even partially) the available space.");

            return measurement;
        }

        internal override void Draw(Size availableSpace)
        {
            foreach (var child in Children)
            {
                if (child.Measure(availableSpace).Type is SpacePlanType.Wrap)
                    continue;
                
                child.Draw(availableSpace);
            }
        }
    }
}
