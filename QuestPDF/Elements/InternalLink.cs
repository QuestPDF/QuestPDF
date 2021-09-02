using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class InternalLink : ContainerElement
    {
        public string LocationName { get; set; }
        
        internal override void Draw(Size availableSpace)
        {
            var targetSize = base.Measure(availableSpace);

            if (targetSize.Type == SpacePlanType.Wrap)
                return;

            Canvas.DrawLocationLink(LocationName, targetSize);
            base.Draw(availableSpace);
        }
    }
}