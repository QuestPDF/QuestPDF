using QuestPDF.Drawing;
using QuestPDF.Elements.Text;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal sealed class RepeatContent : ContainerElement, IStateful
{
    public SemanticTreeManager SemanticTreeManager { get; set; }
    
    public enum RepeatContext
    {
        PageHeader,
        PageFooter,
        Other
    }
    
    public RepeatContext Type { get; set; } = RepeatContext.Other;
    
    internal override void Draw(Size availableSpace)
    {
        OptimizeContentCacheBehavior();
        
        if (IsFullyRendered)
        {
            SemanticTreeManager.BeginArtifactContent();
            
            var paginationNodeId = Type switch
            {
                RepeatContext.PageHeader => SkSemanticNodeSpecialId.PaginationHeaderArtifact,
                RepeatContext.PageFooter => SkSemanticNodeSpecialId.PaginationFooterArtifact,
                _ => SkSemanticNodeSpecialId.PaginationArtifact
            };
        
            Canvas.SetSemanticNodeId(paginationNodeId);
        }

        var childMeasurement = Child?.Measure(availableSpace);
        base.Draw(availableSpace);
        
        if (IsFullyRendered)
            SemanticTreeManager.EndArtifactContent();

        if (childMeasurement?.Type == SpacePlanType.FullRender)
        {
            Child.VisitChildren(x => (x as IStateful)?.ResetState(false));
            IsFullyRendered = true;
        }
    }
    
    #region Text Optimization

    private bool IsContentOptimizationExecuted { get; set; } = false;
    
    /// <summary>
    /// <para>
    /// The TextBlock element uses SkParagraph cache to enhance rendering speed.
    /// This cache uses a significant amount of memory and is cleared after FullRender.
    /// However, when using the RepeatContent element, the cache is cleared after each repetition.
    /// To avoid performance issues, the default behavior is disabled.
    /// </para>
    ///
    /// <para>
    /// Similarly, the Lazy element builds entire content on demand, waits to fully render it and then removes it.
    /// This aims to optimize managed memory usage.
    /// However, it may not be the most optimal solution in repeating context.
    /// </para>
    /// </summary>
    private void OptimizeContentCacheBehavior()
    {
        if (IsContentOptimizationExecuted)
            return;
        
        IsContentOptimizationExecuted = true;
        
        Child.VisitChildren(x =>
        {
            if (x is TextBlock text)
                text.ClearInternalCacheAfterFullRender = false;
            
            if (x is Lazy lazy)
                lazy.ClearCacheAfterFullRender = false;
        });
    }
    
    #endregion
    
    #region IStateful
        
    private bool IsFullyRendered { get; set; }

    public void ResetState(bool hardReset = false)
    {
        if (hardReset)
            IsFullyRendered = false;
    }
    
    public object GetState() => IsFullyRendered;
    public void SetState(object state) => IsFullyRendered = (bool) state;
    
    #endregion
}