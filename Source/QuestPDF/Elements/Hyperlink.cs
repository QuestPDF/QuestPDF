using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Hyperlink : ContainerElement, IContentDirectionAware
    {
        public ContentDirection ContentDirection { get; set; }
        public string Url { get; set; } = "https://www.questpdf.com";
        
        internal override void Draw(Size availableSpace)
        {
            var targetSize = base.Measure(availableSpace);

            if (targetSize.Type == SpacePlanType.Wrap)
                return;
            
            var offset = ContentDirection == ContentDirection.LeftToRight
                ? Position.Zero
                : new Position(availableSpace.Width - targetSize.Width, 0);

            Canvas.Translate(offset);
            Canvas.DrawHyperlink(Url, availableSpace);
            Canvas.Translate(offset.Reverse());
            
            base.Draw(availableSpace);
        }
    }
}