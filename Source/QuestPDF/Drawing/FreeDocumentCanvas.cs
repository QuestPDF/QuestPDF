using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing;

internal class FreeDocumentCanvas : IDocumentCanvas
{
    private FreeDrawingCanvas DrawingCanvas { get; } = new();
        
    public void BeginDocument()
    {
            
    }

    public void EndDocument()
    {
            
    }

    public void BeginPage(Size size)
    {
            
    }

    public void EndPage()
    {
            
    }

    public IDrawingCanvas GetDrawingCanvas()
    {
        return DrawingCanvas;
    }
}