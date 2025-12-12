using QuestPDF.Drawing;

namespace QuestPDF.LayoutTests.TestEngine;

internal sealed class LayoutTestDocumentCanvas : IDocumentCanvas
{
    private LayoutTestDrawingCanvas DrawingCanvas { get; } = new();
        
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