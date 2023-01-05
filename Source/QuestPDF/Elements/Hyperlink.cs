using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Hyperlink : ContainerElement
    {
        public string Url { get; set; } = "https://www.questpdf.com";
        
        internal override void Draw(Size availableSpace)
        {
            var targetSize = base.Measure(availableSpace);

            if (targetSize.Type == SpacePlanType.Wrap)
                return;

            Canvas.DrawHyperlink(Url, targetSize);
            base.Draw(availableSpace);
        }
    }
}