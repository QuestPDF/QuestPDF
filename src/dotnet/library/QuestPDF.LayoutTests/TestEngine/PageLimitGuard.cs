namespace QuestPDF.LayoutTests.TestEngine;

/// <summary>
/// Fails the test when the content generates an unreasonable number of pages.
/// Without it, a layout that is stuck in an infinite loop hangs the entire test run.
/// </summary>
internal class PageLimitGuard : ContainerElement, IPageContextAware
{
    private const int MaxPages = 100;
    
    public IPageContext PageContext { get; set; }
    
    internal override void Draw(Size availableSpace)
    {
        if (PageContext.CurrentPage > MaxPages)
            throw new InvalidOperationException($"The content has not been fully rendered within {MaxPages} pages. The layout is most likely stuck in an infinite loop.");
        
        base.Draw(availableSpace);
    }
}
