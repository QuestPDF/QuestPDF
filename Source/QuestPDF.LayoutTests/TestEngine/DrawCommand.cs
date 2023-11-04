using QuestPDF.Infrastructure;

namespace QuestPDF.LayoutTests.TestEngine;

internal class PageDrawingCommand
{
    public Size RequiredArea { get; set; }
    public ICollection<ChildDrawingCommand> Children { get; set; }
}

internal class ChildDrawingCommand
{
    public string ChildId { get; set; }
    public Position Position { get; set; }
    public Size Size { get; set; }
}