using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.Proxy;

internal class LayoutDrawingSnapshot
{
    public int PageNumber { get; set; }
    public Position Position { get; set; }
    public Size Size { get; set; }
}

internal class LayoutProxy : ElementProxy
{
    public List<LayoutDrawingSnapshot> Snapshots { get; } = new();
    
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

        Snapshots.Add(new LayoutDrawingSnapshot
        {
            PageNumber = PageContext.CurrentPage,
            Position = new Position(position.TranslateX, position.TranslateY),
            Size = availableSpace
        });
    }
}