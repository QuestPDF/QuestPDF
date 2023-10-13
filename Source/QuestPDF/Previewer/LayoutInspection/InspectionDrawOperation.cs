using QuestPDF.Infrastructure;

namespace QuestPDF.Previewer.LayoutInspection;

internal sealed class InspectionDrawOperation
{
    public int PageNumber { get; set; }
    public Size Size { get; set; }
    public Position Position { get; set; }
}