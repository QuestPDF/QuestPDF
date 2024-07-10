using System.Collections.Generic;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing.Proxy;

internal class LayoutProxy : ElementProxy
{
    public List<PageLocation> Snapshots { get; } = new();
    
    public LayoutProxy(Element child)
    {
        Child = child;
    }
    
    internal override void Draw(Size availableSpace)
    {
        base.Draw(availableSpace);
        
        var canvas = Canvas as SkiaCanvasBase;
        
        if (canvas == null)
            return;
        
        var position = canvas.Canvas.GetCurrentTotalMatrix();

        Snapshots.Add(new PageLocation
        {
            PageNumber = PageContext.CurrentPage,
            Left = position.TranslateX,
            Top = position.TranslateY,
            Right = position.TranslateX + availableSpace.Width,
            Bottom = position.TranslateY + availableSpace.Height
        });
    }
}