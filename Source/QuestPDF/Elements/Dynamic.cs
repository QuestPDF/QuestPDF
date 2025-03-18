using System;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class DynamicHost : Element, IStateful, IContentDirectionAware
    {
        private DynamicComponentProxy Child { get; }
        private object InitialComponentState { get; set; }

        internal TextStyle TextStyle { get; set; } = TextStyle.Default;
        public ContentDirection ContentDirection { get; set; }
        
        internal int? ImageTargetDpi { get; set; }
        internal ImageCompressionQuality? ImageCompressionQuality { get; set; }
        internal bool UseOriginalImage { get; set; }
        
        public DynamicHost(DynamicComponentProxy child)
        {
            Child = child;
            
            InitialComponentState = Child.GetState();
        }
 
        internal override SpacePlan Measure(Size availableSpace)
        {
            if (IsRendered)
                return SpacePlan.Empty();
            
            var result = ComposeContent(availableSpace, acceptNewState: false);
            var content = result.Content as Element ?? Empty.Instance;
            var measurement = content.Measure(availableSpace);
            content.ReleaseDisposableChildren();
            
            if (measurement.Type is SpacePlanType.PartialRender or SpacePlanType.Wrap)
                throw new DocumentLayoutException("Dynamic component generated content that does not fit on a single page.");
            
            return result.HasMoreContent 
                ? SpacePlan.PartialRender(measurement) 
                : SpacePlan.FullRender(measurement);
        }

        internal override void Draw(Size availableSpace)
        {
            var composeResult = ComposeContent(availableSpace, acceptNewState: true);
            var content = composeResult.Content as Element; 
            content?.Draw(availableSpace);
            content.ReleaseDisposableChildren();
            
            if (!composeResult.HasMoreContent)
                IsRendered = true;
        }

        private DynamicComponentComposeResult ComposeContent(Size availableSize, bool acceptNewState)
        {
            var componentState = Child.GetState();
            
            var context = new DynamicContext
            {
                PageContext = PageContext,
                Canvas = Canvas,
                
                TextStyle = TextStyle,
                ContentDirection = ContentDirection,
                
                ImageTargetDpi = ImageTargetDpi.Value,
                ImageCompressionQuality = ImageCompressionQuality.Value,
                UseOriginalImage = UseOriginalImage,
                
                PageNumber = PageContext.CurrentPage,
                TotalPages = PageContext.IsInitialRenderingPhase ? int.MaxValue : PageContext.DocumentLength,
                AvailableSize = availableSize
            };
            
            var result = Child.Compose(context);

            if (!acceptNewState)
                Child.SetState(componentState);

            return result;
        }
        
        #region IStateful
        
        private bool IsRendered { get; set; }
    
        public void ResetState(bool hardReset = false)
        {
            IsRendered = false;
            Child.SetState(InitialComponentState);
        }

        public object GetState() => IsRendered;
        public void SetState(object state) => IsRendered = (bool) state;
    
        #endregion
    }

    /// <summary>
    /// Stores all contextual information available for the dynamic component.
    /// </summary>
    public class DynamicContext
    {
        internal IPageContext PageContext { get; set; }
        internal ICanvas Canvas { get; set; }

        internal TextStyle TextStyle { get; set; }
        internal ContentDirection ContentDirection { get; set; }

        internal int ImageTargetDpi { get; set; }
        internal ImageCompressionQuality ImageCompressionQuality { get; set; }
        internal bool UseOriginalImage { get; set; }
        
        /// <summary>
        /// Returns the number of the page being rendered at the moment.
        /// </summary>
        public int PageNumber { get; internal set; }
        
        /// <summary>
        /// Returns the total count of pages in the document.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Document rendering process is performed in two phases.
        /// During the first phase, the value of this property is equal to <c>int.MaxValue</c> to indicate its unavailability.
        /// </para>
        /// <para>Please note that using this property may result with unstable layouts and unpredicted behaviors, especially when generating conditional content of various sizes.</para>
        /// </remarks>
        public int TotalPages { get; internal set; }
        
        /// <summary>
        /// Returns the vertical and horizontal space, in points, available to the dynamic component.
        /// </summary>
        public Size AvailableSize { get; internal set; }

        /// <summary>
        /// Enables the creation of unattached layout structures and provides their size measurements.
        /// </summary>
        /// <param name="content">The handler responsible for constructing the new layout structure.</param>
        /// <returns>A newly created content, with its physical size.</returns>
        public IDynamicElement CreateElement(Action<IContainer> content)
        {
            var container = new DynamicElement();
            content(container);
            
            container.ApplyInheritedAndGlobalTexStyle(TextStyle);
            container.ApplyContentDirection(ContentDirection);
            container.ApplyDefaultImageConfiguration(ImageTargetDpi, ImageCompressionQuality, UseOriginalImage);
            
            container.InjectDependencies(PageContext, Canvas);
            container.VisitChildren(x => (x as IStateful)?.ResetState());

            container.Size = container.Measure(Size.Max);
            
            return container;
        }
    }

    /// <summary>
    /// Represents any unattached content element, created by the dynamic component.
    /// </summary>
    public interface IDynamicElement : IElement
    {
        /// <summary>
        /// Specifies the vertical and horizontal size, measured in points, required by the element to be drawn completely, assuming infinite canvas.
        /// </summary>
        Size Size { get; }
    }

    internal sealed class DynamicElement : ContainerElement, IDynamicElement
    {
        public Size Size { get; internal set; }
    }
}