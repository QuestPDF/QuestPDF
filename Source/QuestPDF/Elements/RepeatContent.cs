using QuestPDF.Drawing;
using QuestPDF.Elements.Text;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal sealed class RepeatContent : ContainerElement
{
    internal override void Draw(Size availableSpace)
    {
        OptimizeTextCacheBehavior();
        
        var childMeasurement = Child?.Measure(availableSpace);
        base.Draw(availableSpace);

        if (childMeasurement?.Type == SpacePlanType.FullRender)
        {
            Child.VisitChildren(x => (x as IStateful)?.ResetState(false));
        }
    }
    
    #region Text Optimization

    private bool IsTextOptimizationExecuted { get; set; } = false;
    
    /// <summary>
    /// The TextBlock element uses SkParagraph cache to enhance rendering speed.
    /// This cache uses a significant amount of memory and is cleared after FullRender.
    /// However, when using the RepeatContent element, the cache is cleared after each repetition.
    /// To avoid performance issues, the default behavior is disabled.
    /// </summary>
    private void OptimizeTextCacheBehavior()
    {
        if (IsTextOptimizationExecuted)
            return;
        
        IsTextOptimizationExecuted = true;
        
        Child.VisitChildren(x =>
        {
            if (x is TextBlock text)
                text.ClearInternalCacheAfterFullRender = false;
        });
    }
    
    #endregion
}