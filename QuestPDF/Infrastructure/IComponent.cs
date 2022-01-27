using System;
using QuestPDF.Elements;

namespace QuestPDF.Infrastructure
{
    interface ISlot
    {
        
    }

    class Slot : Container, ISlot
    {
        
    }

    interface ISlot<T>
    {
        
    }

    class Slot<T> : ISlot<T>
    {
        public Func<T, IElement> GetContent { get; set; }
    }

    public interface IComponent
    {
        void Compose(IContainer container);
    }
}