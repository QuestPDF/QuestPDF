using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class ZIndex : ContainerElement
    {
        public int Depth { get; set; }
        
        internal override void Draw(Size availableSpace)
        {
            var previousMatrix = Canvas.GetCurrentMatrix();
            var previousZIndex = Canvas.GetZIndex();
            Canvas.SetZIndex(Depth);
            Canvas.SetMatrix(previousMatrix);
            
            base.Draw(availableSpace);
            
            var newMatrix = Canvas.GetCurrentMatrix();
            Canvas.SetZIndex(previousZIndex);
            Canvas.SetMatrix(newMatrix);
        }
    }
}