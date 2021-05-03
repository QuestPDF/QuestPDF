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
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            return Children
                .Single(x => x.IsPrimary)
                .Measure(availableSpace);
        }

        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            Children
                .Where(x => x.Measure(availableSpace) is Size)
                .ToList()
                .ForEach(x => x.Draw(canvas, availableSpace));
        }
    }
}