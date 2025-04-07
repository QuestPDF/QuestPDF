using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class ZIndex : ContainerElement
    {
        public int Depth { get; set; }
        
        internal override void Draw(Size availableSpace)
        {
            var previousZIndex = Canvas.GetZIndex();
            
            Canvas.SetZIndex(Depth);
            base.Draw(availableSpace);
            Canvas.SetZIndex(previousZIndex);
        }
    }
}