using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class ExternalLink : ContainerElement
    {
        public string Url { get; set; } = "https://www.questpdf.com";
        
        internal override void Draw(Size availableSpace)
        {
            var targetSize = Child?.Measure(availableSpace) as Size;

            if (targetSize == null)
                return;

            Canvas.DrawExternalLink(Url, targetSize);
            Child?.Draw(availableSpace);
        }
    }
}