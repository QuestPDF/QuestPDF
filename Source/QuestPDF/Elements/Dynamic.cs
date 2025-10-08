using System;
using System.Collections.Generic;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Exceptions;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements
{
    internal sealed class DynamicHost : Element, IStateful, IContentDirectionAware, ISemanticAware
    {
        public SemanticTreeManager SemanticTreeManager { get; set; }
        
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

            var context = CreateContext(availableSpace);
            var result = ComposeContent(context, acceptNewState: false);
            var content = result.Content as Element ?? Empty.Instance;
            var measurement = content.Measure(availableSpace);
            
            context.DisposeCreatedElements();
            content.ReleaseDisposableChildren();
            
            if (measurement.Type is SpacePlanType.PartialRender or SpacePlanType.Wrap)
                throw new DocumentLayoutException("Dynamic component generated content that does not fit on a single page.");
            
            return result.HasMoreContent 
                ? SpacePlan.PartialRender(measurement) 
                : SpacePlan.FullRender(measurement);
        }

        internal override void Draw(Size availableSpace)
        {
            var context = CreateContext(availableSpace);
            var composeResult = ComposeContent(context, acceptNewState: true);
            var content = composeResult.Content as Element; 
            content?.Draw(availableSpace);
            
            context.DisposeCreatedElements();
            content.ReleaseDisposableChildren();
            
            if (!composeResult.HasMoreContent)
                IsRendered = true;
        }

        private DynamicContext CreateContext(Size availableSize)
        {
            return new DynamicContext
            {
                PageContext = PageContext,
                Canvas = Canvas,
                SemanticTreeManager = SemanticTreeManager,
                
                TextStyle = TextStyle,
                ContentDirection = ContentDirection,
                
                ImageTargetDpi = ImageTargetDpi.Value,
                ImageCompressionQuality = ImageCompressionQuality.Value,
                UseOriginalImage = UseOriginalImage,
                
                PageNumber = PageContext.CurrentPage,
                TotalPages = PageContext.IsInitialRenderingPhase ? int.MaxValue : PageContext.DocumentLength,
                AvailableSize = availableSize
            };
        }
        
        private DynamicComponentComposeResult ComposeContent(DynamicContext context, bool acceptNewState)
        {
            var componentState = Child.GetState();
            
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
    public sealed class DynamicContext
    {
        internal IPageContext PageContext { get; set; }
        internal IDrawingCanvas Canvas { get; set; }
        internal SemanticTreeManager SemanticTreeManager { get; set; }
        
        internal TextStyle TextStyle { get; set; }
        internal ContentDirection ContentDirection { get; set; }

        internal int ImageTargetDpi { get; set; }
        internal ImageCompressionQuality ImageCompressionQuality { get; set; }
        internal bool UseOriginalImage { get; set; }
        
        internal List<Element> CreatedElements { get; } = new();
        
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
        /// Returns all page locations of the captured element.
        /// </summary>
        public ICollection<PageElementLocation> GetContentCapturedPositions(string id)
        {
            return PageContext.GetContentCapturedPositions(id);
        }

        /// <summary>
        /// Enables the creation of unattached layout structures and provides their size measurements.
        /// </summary>
        /// <param name="content">The handler responsible for constructing the new layout structure.</param>
        /// <returns>A newly created content, with its physical size.</returns>
        public IDynamicElement CreateElement(Action<IContainer> content)
        {
            var container = new DynamicElement();
            CreatedElements.Add(container);
            content(container);
            
            container.ApplyInheritedAndGlobalTexStyle(TextStyle);
            container.ApplyContentDirection(ContentDirection);
            container.ApplyDefaultImageConfiguration(ImageTargetDpi, ImageCompressionQuality, UseOriginalImage);
            container.ApplySemanticParagraphs();
            
            container.InjectDependencies(PageContext, Canvas);
            container.InjectSemanticTreeManager(SemanticTreeManager);
            container.VisitChildren(x => (x as IStateful)?.ResetState());

            container.Size = container.Measure(Size.Max);
            
            return container;
        }
        
        internal void DisposeCreatedElements()
        {
            foreach (var element in CreatedElements)
                element.ReleaseDisposableChildren();
            
            CreatedElements.Clear();
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