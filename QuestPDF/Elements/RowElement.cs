using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal abstract class RowElement : ContainerElement
    {
        public float Width { get; set; } = 1;
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            Child?.Draw(canvas, availableSpace);
        }
    }
    
    internal class ConstantRowElement : RowElement
    {
        public ConstantRowElement(float width)
        {
            Width = width;
        }
    }
    
    internal class RelativeRowElement : RowElement
    {
        public RelativeRowElement(float width)
        {
            Width = width;
        }
    }
}