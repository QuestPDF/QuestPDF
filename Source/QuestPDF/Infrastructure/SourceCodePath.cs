using System.Diagnostics;
using System.Linq;
using QuestPDF.Previewer;

namespace QuestPDF.Infrastructure;

internal readonly struct SourceCodePath(StackFrame frame)
{
    public readonly string FilePath = frame.GetFileName() ?? string.Empty;
    public readonly int LineNumber = frame.GetFileLineNumber();

    internal static SourceCodePath? CreateFromCurrentStackTrace()
    {
        if (!PreviewerService.IsPreviewerAttached)
            return null;

        var stackTrace = new StackTrace(true);

        var frame = stackTrace.GetFrames().FirstOrDefault(x => x.HasSource() && x.HasMethod());

        if (frame == null)
            return null;

        return new SourceCodePath(frame);
    }
}