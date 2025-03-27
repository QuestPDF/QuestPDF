namespace QuestPDF.Infrastructure
{
    internal interface IDocumentCanvas
    {
        bool DocumentContentHasLayoutOverflowIssues { get; set; }
        void MarkCurrentPageAsHavingLayoutIssues();
        
        void BeginDocument();
        void EndDocument();
        
        void BeginPage(Size size);
        void EndPage();
    }
}