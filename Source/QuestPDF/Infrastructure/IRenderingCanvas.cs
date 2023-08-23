namespace QuestPDF.Infrastructure
{
    internal interface IRenderingCanvas
    {
        void BeginDocument();
        void EndDocument();
        
        void BeginPage(Size size);
        void EndPage();
    }
}