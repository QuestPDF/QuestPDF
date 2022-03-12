using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class SectionLink : ContainerElement
    {
        public string SectionName { get; set; }
        
        internal override void Draw(Size availableSpace)
        {
            var targetSize = base.Measure(availableSpace);

            if (targetSize.Type == SpacePlanType.Wrap)
                return;

            Canvas.DrawSectionLink(SectionName, targetSize);
            base.Draw(availableSpace);
        }
    }
}