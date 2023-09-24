using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal class CanvasCacheProxy : ElementProxy
{
    private SkiaPictureCanvas PictureCanvas { get; set; } = new();
    private int? FirstPageIndex { get; set; }
    
    public CanvasCacheProxy(Element child)
    {
        Child = child;
    }
    
    internal override SpacePlan Measure(Size availableSpace)
    {
        var snapshotIndex = PageContext.CurrentPage - FirstPageIndex;

        if (snapshotIndex > 0 && snapshotIndex < PictureCanvas.Pictures.Count)
        {
            var isLast = snapshotIndex == PictureCanvas.Pictures.Count - 1;
            var size = PictureCanvas.Pictures[snapshotIndex.Value].Size;

            return isLast ? SpacePlan.FullRender(size) : SpacePlan.PartialRender(size);
        }
        
        return Child?.Measure(availableSpace) ?? SpacePlan.FullRender(0, 0);
    }
        
    internal override void Draw(Size availableSpace)
    {
        var snapshotIndex = PageContext.CurrentPage - FirstPageIndex;

        if (snapshotIndex > 0 && snapshotIndex < PictureCanvas.Pictures.Count)
        {
            var picture = PictureCanvas.Pictures[snapshotIndex.Value];
            Canvas.DrawPicture(picture.Picture);
            return;
        }
        
        FirstPageIndex ??= PageContext.CurrentPage;
 
        PictureCanvas.BeginPage(availableSpace);
        Child.VisitChildren(x => x.Canvas = PictureCanvas);
        Child?.Draw(availableSpace);
        PictureCanvas.EndPage();
    }
}