using System;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal class DynamicHost : Element, IStateResettable, IContentDirectionAware
    {
        private DynamicComponentProxy Child { get; }
        private object InitialComponentState { get; set; }

        internal TextStyle TextStyle { get; set; } = TextStyle.Default;
        public ContentDirection ContentDirection { get; set; }
        
        public DynamicHost(DynamicComponentProxy child)
        {
            Child = child;
            
            InitialComponentState = Child.GetState();
        }

        public void ResetState()
        {
            Child.SetState(InitialComponentState);
        }
        
        internal override SpacePlan Measure(Size availableSpace)
        {
            var result = GetContent(availableSpace, acceptNewState: false);
            var content = result.Content as Element ?? Empty.Instance;
            var measurement = content.Measure(availableSpace);

            if (measurement.Type != SpacePlanType.FullRender)
                throw new DocumentLayoutException("Dynamic component generated content that does not fit on a single page.");
            
            return result.HasMoreContent 
                ? SpacePlan.PartialRender(measurement) 
                : SpacePlan.FullRender(measurement);
        }

        internal override void Draw(Size availableSpace)
        {
            var content = GetContent(availableSpace, acceptNewState: true).Content as Element; 
            content?.Draw(availableSpace);
        }

        private DynamicComponentComposeResult GetContent(Size availableSize, bool acceptNewState)
        {
            var componentState = Child.GetState();
            
            var context = new DynamicContext
            {
                PageContext = PageContext,
                Canvas = Canvas,
                
                TextStyle = TextStyle,
                ContentDirection = ContentDirection,
                
                PageNumber = PageContext.CurrentPage,
                TotalPages = PageContext.GetLocation(Infrastructure.PageContext.DocumentLocation).PageEnd,
                AvailableSize = availableSize
            };
            
            var result = Child.Compose(context);

            if (!acceptNewState)
                Child.SetState(componentState);

            return result;
        }
    }

    public class DynamicContext
    {
        internal IPageContext PageContext { get; set; }
        internal ICanvas Canvas { get; set; }
        
        internal TextStyle TextStyle { get; set; }
        internal ContentDirection ContentDirection { get; set; }
    
        public int PageNumber { get; internal set; }
        public int TotalPages { get; internal set; }
        public Size AvailableSize { get; internal set; }

        public IDynamicElement CreateElement(Action<IContainer> content)
        {
            var container = new DynamicElement();
            content(container);
            
            container.ApplyInheritedAndGlobalTexStyle(TextStyle);
            container.ApplyContentDirection(ContentDirection);
            
            container.InjectDependencies(PageContext, Canvas);
            container.VisitChildren(x => (x as IStateResettable)?.ResetState());

            container.Size = container.Measure(Size.Max);
            
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