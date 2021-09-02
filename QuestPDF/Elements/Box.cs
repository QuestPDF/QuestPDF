using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Box : ContainerElement
    {
        internal override void Draw(Size availableSpace)
        {
            var targetSize = base.Measure(availableSpace);
            
            if (targetSize.Type == SpacePlanType.Wrap)
                return;
            
            base.Draw(targetSize);
        }
    }
}