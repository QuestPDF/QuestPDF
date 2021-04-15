using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class EnsureSpace : ContainerElement
    {
        public const float DefaultMinHeight = 150;
        public float MinHeight { get; set; } = DefaultMinHeight;
        
        internal override ISpacePlan Measure(Size availableSpace)
        {
            if (availableSpace.Height < MinHeight)
                return new Wrap();

            return base.Measure(availableSpace);
        }
    }
}