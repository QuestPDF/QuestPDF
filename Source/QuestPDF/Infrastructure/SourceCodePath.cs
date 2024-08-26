using System.Diagnostics;
using System.Linq;
using QuestPDF.Companion;

namespace QuestPDF.Infrastructure;

internal readonly struct SourceCodePath(StackFrame frame)
{
    public readonly string FilePath = frame.GetFileName() ?? string.Empty;
    public readonly int LineNumber = frame.GetFileLineNumber();

    internal static SourceCodePath? CreateFromCurrentStackTrace()
    {
        #if NET6_0_OR_GREATER
        
        if (!CompanionService.IsCompanionAttached)
            return null;

        #endif

        var stackTrace = new StackTrace(true);

        var frame = stackTrace.GetFrames().FirstOrDefault(x => x.HasSource() && x.HasMethod());

        if (frame == null)
            return null;

        return new SourceCodePath(frame);
    }
}