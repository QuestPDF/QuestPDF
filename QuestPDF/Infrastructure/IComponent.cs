using QuestPDF.Elements;

namespace QuestPDF.Infrastructure
{
    interface ISlot
    {
        
    }

    class Slot : ContainerElement, ISlot
    {
        
    }
    
    public interface IComponent
    {
        void Compose(IContainer container);
    }
}