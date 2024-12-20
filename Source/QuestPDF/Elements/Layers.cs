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
        
        internal override IEnumerable<Element?> GetChildren()
        {
            return Children;
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var measurement = Children
                .Single(x => x.IsPrimary)
                .Measure(availableSpace);

            if (measurement.Type == SpacePlanType.Wrap)
                return SpacePlan.Wrap("The content of the primary layer does not fit (even partially) the available space.");

            return measurement;
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