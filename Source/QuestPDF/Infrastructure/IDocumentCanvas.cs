using QuestPDF.Drawing;

namespace QuestPDF.Infrastructure
{
    internal interface IDocumentCanvas
    {
        void SetSemanticTree(SemanticTreeNode? semanticTree);
        
        void BeginDocument();
        void EndDocument();
        
        void BeginPage(Size size);
        void EndPage();
        
        IDrawingCanvas GetDrawingCanvas();
    }
}