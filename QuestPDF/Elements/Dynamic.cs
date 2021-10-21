using System;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class DynamicHost : Element, IStateResettable
    {
        private IDynamicComponent Child { get; }

        public DynamicHost(IDynamicComponent child)
        {
            Child = child;
        }

        public void ResetState()
        {
            GetContent(Size.Zero, DynamicLayoutOperation.Reset);
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var content = GetContent(availableSpace, DynamicLayoutOperation.Measure);
            var measurement = content.Measure(availableSpace);

            if (measurement.Type != SpacePlanType.FullRender)
                throw new DocumentLayoutException("Dynamic component generated content that does not fit on a single page.");
            
            return content.HasMoreContent 
                ? SpacePlan.PartialRender(measurement) 
                : SpacePlan.FullRender(measurement);
        }

        internal override void Draw(Size availableSpace)
        {
            GetContent(availableSpace, DynamicLayoutOperation.Draw).Draw(availableSpace);
        }

        DynamicContainer GetContent(Size availableSize, DynamicLayoutOperation operation)
        {
            var context = new DynamicContext
            {
                PageNumber = PageContext.GetLocationPage(Infrastructure.PageContext.CurrentPageSlot),
                PageContext = PageContext,
                Canvas = Canvas,
                
                AvailableSize = availableSize,
                Operation = operation
            };
            
            var container = new DynamicContainer();
            Child.Compose(context, container);
            
            container.HandleVisitor(x => x?.Initialize(PageContext, Canvas));
            container.HandleVisitor(x => (x as IStateResettable)?.ResetState());
            
            return container;
        }
    }

    public enum DynamicLayoutOperation
    {
        Reset,
        Measure,
        Draw
    }
    
    public class DynamicContext
    {
        internal IPageContext PageContext { get; set; }
        internal ICanvas Canvas { get; set; }
        
        public int PageNumber { get; internal set; }
        public Size AvailableSize { get; internal set; }
        public DynamicLayoutOperation Operation { get; internal set; }
        
        public IDynamicElement CreateElement(Action<IContainer> content)
        {
            var container = new DynamicElement();
            content(container);
            
            container.HandleVisitor(x => x?.Initialize(PageContext, Canvas));
            container.HandleVisitor(x => (x as IStateResettable)?.ResetState());

            container.Size = container.Measure(AvailableSize);
            
            return container;
        }
    }

    public interface IDynamicContainer : IContainer
    {
        
    }

    internal class DynamicContainer : Container, IDynamicContainer
    {
        internal bool HasMoreContent { get; set; }
    }

    public static class DynamicContainerExtensions
    {
        public static IDynamicContainer HasMoreContent(this IDynamicContainer container)
        {
            (container as DynamicContainer).HasMoreContent = true;
            return container;
        }
    }
    
    public interface IDynamicElement : IElement
    {
        Size Size { get; }
    }

    internal class DynamicElement : ContainerElement, IDynamicElement
    {
        public Size Size { get; internal set; }
    }
}