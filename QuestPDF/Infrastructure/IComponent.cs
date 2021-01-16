using QuestPDF.Elements;

namespace QuestPDF.Infrastructure
{
    interface ISlot
    {
        
    }

    class Slot : Container, ISlot
    {
        
    }
    
    public interface IComponent
    {
        void Compose(IContainer container);
    }
}