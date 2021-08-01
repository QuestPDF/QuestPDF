using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class Box : ContainerElement
    {
        internal override void Draw(Size availableSpace)
        {
            var targetSize = Child?.Measure(availableSpace) as Size;
            
            if (targetSize == null)
                return;
            
            Child?.Draw(targetSize);
        }
    }
}