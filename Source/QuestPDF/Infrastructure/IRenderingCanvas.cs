namespace QuestPDF.Infrastructure
{
    internal interface IRenderingCanvas
    {
        bool DocumentContentHasLayoutOverflowIssues { get; set; }
        
        void BeginDocument();
        void EndDocument();
        
        void BeginPage(Size size);
        void EndPage();
    }
}