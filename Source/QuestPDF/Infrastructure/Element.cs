using System;
using System.Collections.Generic;
using QuestPDF.Drawing;

namespace QuestPDF.Infrastructure
{
    internal abstract class Element : IElement
    {
        internal ICanvas Canvas { get; set; }
        
        internal virtual IEnumerable<Element?> GetChildren()
        {
            yield break;
        }

        internal virtual void CreateProxy(Func<Element?, Element?> create)
        {
            
        }
        
        internal abstract SpacePlan Measure(Size availableSpace);
        internal abstract void Draw(Size availableSpace);
    }
}