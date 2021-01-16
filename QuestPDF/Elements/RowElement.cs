using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal abstract class RowElement : ContainerElement
    {
        public float Width { get; set; } = 1;
        
        internal override void Draw(ICanvas canvas, Size availableSpace)
        {
            if (Child == null)
                return;
            
            Child.Draw(canvas, availableSpace);
        }
    }
    
    internal class ConstantRowElement : RowElement
    {
        
    }
    
    internal class RelativeRowElement : RowElement
    {
        
    }
}