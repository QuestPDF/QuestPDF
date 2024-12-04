using System;
using QuestPDF.Drawing;
using QuestPDF.Drawing.Proxy;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal sealed class Lazy : ContainerElement, IContentDirectionAware, IStateful
{
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
        
        PopulateContent();
        
        var isFullyRendered = Child?.Measure(availableSpace).Type == SpacePlanType.FullRender;
        Child?.Draw(availableSpace);
        
        if (isFullyRendered && ClearCacheAfterFullRender)
        {
            IsRendered = true;
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
        
        container.ApplyInheritedAndGlobalTexStyle(TextStyle);
        container.ApplyContentDirection(ContentDirection);
        container.ApplyDefaultImageConfiguration(ImageTargetDpi.Value, ImageCompressionQuality.Value, UseOriginalImage);
            
        container.InjectDependencies(PageContext, Canvas);
        container.VisitChildren(x => (x as IStateful)?.ResetState());
    }
    
    #region IStateful
        
    private bool IsRendered { get; set; }

    public void ResetState(bool hardReset = false)
    {
        if (hardReset)
            IsRendered = false;
    }
        
    public object GetState() => IsRendered;
    public void SetState(object state) => IsRendered = (bool) state;
    
    #endregion
}