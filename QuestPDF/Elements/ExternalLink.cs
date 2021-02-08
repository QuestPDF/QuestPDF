using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class ExternalLink : ContainerElement
    {
        public string Url { get; set; } = "https://www.questpdf.com";
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var targetSize = Child?.Measure(availableSpace) as Size;

            if (targetSize == null)
                return;

            canvas.DrawExternalLink(Url, targetSize);
            Child?.Draw(canvas, availableSpace);
        }
    }
}