using System;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF.Elements;

internal class MarkRepeatedContentAsArtifact : ContainerElement, IStateful
{
    public PaginationType Type { get; set; } = PaginationType.Other;
    
    public enum PaginationType
    {
        Other,
        Header,
        Footer
    }
    
    internal override void Draw(Size availableSpace)
    {
        if (IsFirstPageRendered)
        {
            Canvas.MarkCurrentContentAsArtifact(true);
        }
        else
        {
            var paginationNodeId = Type switch
            {
                PaginationType.Header => SkSemanticNodeSpecialId.PaginationHeaderArtifact,
                PaginationType.Footer => SkSemanticNodeSpecialId.PaginationFooterArtifact,
                _ => SkSemanticNodeSpecialId.PaginationArtifact
            };
        
            Canvas.SetSemanticNodeId(paginationNodeId);
        }
        
        base.Draw(availableSpace);
        
        if (IsFirstPageRendered)
            Canvas.MarkCurrentContentAsArtifact(false);
        
        IsFirstPageRendered = true;
    }
    
    #region IStateful
        
    private bool IsFirstPageRendered { get; set; }

    public void ResetState(bool hardReset = false)
    {
        if (hardReset)
            IsFirstPageRendered = false;
    }
    
    public object GetState() => IsFirstPageRendered;
    public void SetState(object state) => IsFirstPageRendered = (bool) state;
    
    #endregion
}