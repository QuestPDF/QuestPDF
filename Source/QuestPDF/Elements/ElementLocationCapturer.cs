using QuestPDF.Drawing;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class ElementLocationCapturer : ContainerElement, IContentDirectionAware
{
    public ContentDirection ContentDirection { get; set; }
    
    public string Id { get; set; }
    
    internal override void Draw(Size availableSpace)
    {
        base.Draw(availableSpace);
        
        if (!PageContext.IsInitialRenderingPhase)
            return;

        var matrix = Canvas.GetCurrentMatrix();

        var position = new PageElementLocation
        {
            Id = Id,
            
            PageNumber = PageContext.CurrentPage,
            
            Width = availableSpace.Width,
            Height = availableSpace.Height,
            
            X = ContentDirection == ContentDirection.LeftToRight ? matrix.TranslateX : matrix.TranslateX - availableSpace.Width,
            Y = matrix.TranslateY,
        };
        
        PageContext.CaptureContentPosition(position);
    }
}