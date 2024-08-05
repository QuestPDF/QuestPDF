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
        var size = Child?.Measure(availableSpace) ?? SpacePlan.Empty();

        var position = new PageElementLocation
        {
            Id = Id,
            
            PageNumber = PageContext.CurrentPage,
            
            Width = size.Width,
            Height = size.Height,
            
            X = ContentDirection == ContentDirection.LeftToRight ? matrix.TranslateX : matrix.TranslateX - size.Width,
            Y = matrix.TranslateY,
        };
        
        PageContext.CaptureContentPosition(position);
    }
}