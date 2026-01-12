using System;
using QuestPDF.Drawing;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal sealed class Lazy : ContainerElement, ISemanticAware, IContentDirectionAware, IStateful
{
    public SemanticTreeManager? SemanticTreeManager { get; set; }
    private SemanticTreeSnapshots? SemanticTreeSnapshots { get; set; }
    
    public Action<IContainer> ContentSource { get; set; }
    public bool IsCacheable { get; set; }

    internal TextStyle TextStyle { get; set; } = TextStyle.Default;
    public ContentDirection ContentDirection { get; set; }
        
    internal int? ImageTargetDpi { get; set; }
    internal ImageCompressionQuality? ImageCompressionQuality { get; set; }
    internal bool UseOriginalImage { get; set; }

    internal bool ClearCacheAfterFullRender { get; set; } = true;
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (IsRendered)
            return SpacePlan.Empty();
        
        PopulateContent();
        return Child.Measure(availableSpace);
    }
        
    internal override void Draw(Size availableSpace)
    {
        if (IsRendered)
            return;
        
        SemanticTreeSnapshots ??= new SemanticTreeSnapshots(SemanticTreeManager, PageContext);
        using var scope = SemanticTreeSnapshots.StartSemanticStateScope(RenderCount);
        
        PopulateContent();
        
        var isFullyRendered = Child?.Measure(availableSpace).Type == SpacePlanType.FullRender;
        Child?.Draw(availableSpace);
        RenderCount++;
        
        if (isFullyRendered && ClearCacheAfterFullRender)
        {
            IsRendered = true;
            Child.ReleaseDisposableChildren();
            Child = Empty.Instance;
        }
    }
    
    private void PopulateContent()
    {
        if (Child is not Empty)
            return;
        
        var container = new Container();
        Child = container;
        ContentSource(container);
        
        if (SemanticTreeManager != null)
        {
            container.ApplySemanticParagraphs();
            container.InjectSemanticTreeManager(SemanticTreeManager);
        }
        
        container.ApplyInheritedAndGlobalTexStyle(TextStyle);
        container.ApplyContentDirection(ContentDirection);
        container.ApplyDefaultImageConfiguration(ImageTargetDpi.Value, ImageCompressionQuality.Value, UseOriginalImage);
        container.InjectDependencies(PageContext, Canvas);
        container.VisitChildren(x => (x as IStateful)?.ResetState());
    }
    
    #region IStateful
        
    public struct LazyState
    {
        public int RenderCount;
        public bool IsRendered;
    }

    private int RenderCount { get; set; } 
    private bool IsRendered { get; set; }

    public void ResetState(bool hardReset = false)
    {
        if (hardReset)
        {
            IsRendered = false;
            RenderCount = 0;
        }
    }

    public object GetState()
    {
        return new LazyState
        {
            RenderCount = RenderCount,
            IsRendered = IsRendered
        };
    }

    public void SetState(object state)
    {
        var lazyState = (LazyState) state;
        
        RenderCount = lazyState.RenderCount;
        IsRendered = lazyState.IsRendered;
    }
    
    #endregion
}