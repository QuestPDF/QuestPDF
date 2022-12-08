using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy
{
    internal class DebuggingProxy : ElementProxy
    {
        internal List<MeasureDetails> Measurements { get; } = new();

        public DebuggingProxy(Element child)
        {
            Child = child;
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var spacePlan = Child.Measure(availableSpace);
            Measurements.Add(new MeasureDetails(availableSpace, spacePlan));
            return spacePlan;
        }
    }

    internal struct MeasureDetails
    {
        public Size AvailableSpace { get; }
        public SpacePlan SpacePlan { get;}

        public MeasureDetails(Size availableSpace, SpacePlan spacePlan)
        {
            AvailableSpace = availableSpace;
            SpacePlan = spacePlan;
        }
    }
}