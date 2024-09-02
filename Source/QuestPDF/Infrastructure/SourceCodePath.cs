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

        // for dotnet 6, 7, 8:
        // - after hot-reload, the stack trace does not contain correct source code path
        // - the operation of collecting stack trace slows down generation process significantly
        // TODO: revise for dotnet 9
        if (CompanionService.IsDocumentHotReloaded)
            return null;

        #endif

        var stackTrace = new StackTrace(true);

        var frame = stackTrace.GetFrames().FirstOrDefault(x => x.HasSource() && x.HasMethod());

        if (frame == null)
            return null;

        return new SourceCodePath(frame);
    }
}