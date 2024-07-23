using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SourceCodePointer : ContainerElement
{
    public string MethodName { get; set; }
    public string CalledFrom { get; set; }
    public string SourceFilePath { get; set; }
    public int SourceLineNumber { get; set; }
}
