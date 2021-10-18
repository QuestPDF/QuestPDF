using System;
using System.Collections;
using System.Collections.Generic;
using QuestPDF.Drawing;

namespace QuestPDF.Infrastructure
{
    internal abstract class Element : IElement
    {
        internal IPageContext PageContext { get; set; }
        internal ICanvas Canvas { get; set; }
        
        internal virtual void HandleVisitor(Action<Element?> visit)
        {
            visit(this);
        }

        internal void Initialize(IPageContext pageContext, ICanvas canvas)
        {
            PageContext = pageContext;
            Canvas = canvas;
        }

        internal virtual void CreateProxy(Func<Element?, Element?> create)
        {
            
        }
        
        internal abstract SpacePlan Measure(Size availableSpace);
        internal abstract void Draw(Size availableSpace);
    }
}