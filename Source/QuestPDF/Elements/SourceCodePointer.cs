using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SourceCodePointer : ContainerElement
{
    public string HandlerName { get; set; }
    public string ParentName { get; set; }
    public string SourceFilePath { get; set; }
    public int SourceLineNumber { get; set; }
}
