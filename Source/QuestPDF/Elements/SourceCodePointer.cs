using System.Collections.Generic;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal sealed class SourceCodePointer : ContainerElement
{
    public string MethodName { get; set; }
    public string CalledFrom { get; set; }
    public string FilePath { get; set; }
    public int LineNumber { get; set; }
    
    internal override string? GetCompanionSearchableContent() => $"{MethodName} {CalledFrom} {FilePath}";

    internal override IEnumerable<KeyValuePair<string, string>>? GetCompanionProperties()
    {
        yield return new("MethodName", MethodName);
        yield return new("CalledFrom", CalledFrom);
        yield return new("FilePath", FilePath);
        yield return new("LineNumber", LineNumber.ToString());
    }
}
