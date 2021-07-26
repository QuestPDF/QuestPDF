using System;
using QuestPDF.Drawing.SpacePlan;
using QuestPDF.Elements;

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
        
        internal abstract ISpacePlan Measure(Size availableSpace);
        internal abstract void Draw(Size availableSpace);
    }
}