using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class InternalLink : ContainerElement
    {
        public string LocationName { get; set; }
        
        internal override void Draw(Size availableSpace)
        {
            var targetSize = Child?.Measure(availableSpace) as Size;

            if (targetSize == null)
                return;

            Canvas.DrawLocationLink(LocationName, targetSize);
            Child?.Draw(availableSpace);
        }
    }
}