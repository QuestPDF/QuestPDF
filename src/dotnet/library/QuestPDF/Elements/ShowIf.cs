using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

public sealed class ShowIfContext
{
    public int PageNumber { get; internal set; }
    
    /// <summary>
    /// Returns the total count of pages in the document.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Document rendering process is performed in two phases.
    /// During the first phase, the value of this property is equal to <c>null</c> to indicate its unavailability.
    /// </para>
    /// <para>Please note that using this property may result with unstable layouts and unpredicted behaviors, especially when generating conditional content of various sizes.</para>
    /// </remarks>
    public int? TotalPages { get; internal set; }
}

internal sealed class ShowIf : ContainerElement, IPageContextAware
{
    public IPageContext PageContext { get; set; }
    
    public Predicate<ShowIfContext> VisibilityPredicate { get; set; }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        // hidden content is measured exactly as ShowIf(false), so only the content after ShowIf is hidden,
        // while the elements before it (e.g. padding, border, height) keep their size
        if (!CheckVisibility())
            return Empty.Instance.Measure(availableSpace);

        return base.Measure(availableSpace);
    }
    
    internal override void Draw(Size availableSpace)
    {
        if (CheckVisibility())
            base.Draw(availableSpace);
    }

    private bool CheckVisibility()
    {
        var context = new ShowIfContext
        {
            PageNumber = PageContext.CurrentPage,
            TotalPages = PageContext.IsInitialRenderingPhase ? null : PageContext.DocumentLength
        };

        return VisibilityPredicate(context);
    }
}