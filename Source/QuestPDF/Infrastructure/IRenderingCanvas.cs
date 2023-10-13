using QuestPDF.Previewer.LayoutInspection;

namespace QuestPDF.Infrastructure
{
    internal interface IRenderingCanvas
    {
        DocumentInspectionElement? DocumentInspectionHierarchy { get; set; }

        void BeginDocument();
        void EndDocument();
        
        void BeginPage(Size size);
        void EndPage();
    }
}