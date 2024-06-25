using System;
using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

public class ShowIfContext
{
    public int PageNumber { get; internal set; }
    public int TotalPages { get; internal set; }
}

internal class ShowIf : ContainerElement
{
    public Predicate<ShowIfContext> VisibilityPredicate { get; set; }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        if (!CheckVisibility())
            return SpacePlan.Empty();

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
            TotalPages = PageContext.DocumentLength
        };

        return VisibilityPredicate(context);
    }
}