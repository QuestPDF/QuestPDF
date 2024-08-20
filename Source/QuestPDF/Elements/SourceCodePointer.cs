using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal class SourceCodePointer : ContainerElement
{
    public string MethodName { get; set; }
    public string CalledFrom { get; set; }
    public string FilePath { get; set; }
    public int LineNumber { get; set; }
}
