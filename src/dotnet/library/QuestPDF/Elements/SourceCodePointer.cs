using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using QuestPDF.Infrastructure;

namespace QuestPDF.Elements;

internal sealed class SourceCodePointer : ContainerElement
{
    public string MethodName { get; set; }
    public string CalledFrom { get; set; }
    public string FilePath { get; set; }
    public int LineNumber { get; set; }
    
    internal override string? GetCompanionSearchableContent() => $"{DisplayMethodName} {CalledFrom} {FilePath}";

    internal override string? GetCompanionHint() => $"{DisplayMethodName} in {Path.GetFileName(FilePath)}:{LineNumber}";

    internal override IEnumerable<KeyValuePair<string, string>>? GetCompanionProperties()
    {
        yield return new("MethodName", DisplayMethodName);
        yield return new("CalledFrom", CalledFrom);
        yield return new("FilePath", FilePath);
        yield return new("LineNumber", LineNumber.ToString());
    }
    
    #region DisplayMethodName
    
    private static readonly Regex FirstInvocation = new(@"(\w+)\s*\(", RegexOptions.Compiled);
    private static readonly ConcurrentDictionary<string, string> DisplayNameCache = new();

    internal string DisplayMethodName => string.IsNullOrWhiteSpace(MethodName) 
        ? MethodName 
        : DisplayNameCache.GetOrAdd(MethodName, ExtractMethodName);

    private static string ExtractMethodName(string expression)
    {
        var match = FirstInvocation.Match(expression);
        return match.Success ? match.Groups[1].Value : expression;
    }
    
    #endregion
}
