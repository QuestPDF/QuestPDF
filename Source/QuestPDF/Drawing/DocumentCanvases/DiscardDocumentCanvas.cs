using QuestPDF.Drawing.DrawingCanvases;
using QuestPDF.Infrastructure;

namespace QuestPDF.Drawing.DocumentCanvases;

internal sealed class DiscardDocumentCanvas : IDocumentCanvas
{
    private DiscardDrawingCanvas DrawingCanvas { get; } = new();
        
    public void SetSemanticTree(SemanticTreeNode? semanticTree)
    {
            
    }
    
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