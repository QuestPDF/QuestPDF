using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class InternalLink : ContainerElement
    {
        public string LocationName { get; set; }
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            var targetSize = Child?.Measure(availableSpace) as Size;

            if (targetSize == null)
                return;

            canvas.DrawLocationLink(LocationName, targetSize);
            Child?.Draw(canvas, availableSpace);
        }
    }
}