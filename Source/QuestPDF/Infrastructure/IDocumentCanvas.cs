namespace QuestPDF.Infrastructure
{
    internal interface IDocumentCanvas
    {
        void BeginDocument();
        void EndDocument();
        
        void BeginPage(Size size);
        void EndPage();
        IDrawingCanvas GetDrawingCanvas();
    }
}