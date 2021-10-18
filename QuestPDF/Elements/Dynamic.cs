using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class DynamicHost : Element, IStateResettable
    {
        private IDynamic Child { get; }

        public DynamicHost(IDynamic child)
        {
            Child = child;
        }

        public void ResetState()
        {
            Child.Reset();
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var content = GetContent(availableSpace, false);
            var measurement = content.element.Measure(availableSpace);

            if (measurement.Type == SpacePlanType.FullRender)
                return content.hasMore ? SpacePlan.PartialRender(measurement) : measurement;

            return measurement;
        }

        internal override void Draw(Size availableSpace)
        {
            GetContent(availableSpace, true).element.Draw(availableSpace);
        }

        (Element element, bool hasMore) GetContent(Size availableSize, bool isDrawState)
        {
            var context = new DynamicContext
            {
                PageContext = PageContext,
                Canvas = Canvas,
                
                AvailableSize = availableSize,
                IsDrawStep = isDrawState
            };
            
            var container = new Container();
            var hasMore = Child.Compose(context, container);
            
            container.HandleVisitor(x => x?.Initialize(PageContext, Canvas));
            container.HandleVisitor(x => (x as IStateResettable)?.ResetState());
            
            return (container, hasMore);
        }
    }
    
    public class DynamicContext
    {
        internal IPageContext PageContext { get; set; }
        internal ICanvas Canvas { get; set; }
        
        public Size AvailableSize { get; internal set; }
        public bool IsDrawStep { get; internal set; }
        
        public IDynamicElement Content(Action<IContainer> content)
        {
            var container = new DynamicElement(() => AvailableSize);
            content(container);
            
            container.HandleVisitor(x => x?.Initialize(PageContext, Canvas));
            container.HandleVisitor(x => (x as IStateResettable)?.ResetState());
            
            return container;
        }
    }

    public interface IDynamicElement : IElement
    {
        Size Measure();
    }

    internal class DynamicElement : ContainerElement, IDynamicElement
    {
        private Func<Size> AvailableSizeSource { get; }

        public DynamicElement(Func<Size> availableSizeSource)
        {
            AvailableSizeSource = availableSizeSource;
        }
        
        Size IDynamicElement.Measure()
        {
            return Measure(AvailableSizeSource());
        }
    }
}