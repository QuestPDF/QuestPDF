using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Previewer;

namespace QuestPDF.Infrastructure
{
    internal abstract class Element : IElement
    {
        internal IPageContext PageContext { get; set; }
        internal ICanvas Canvas { get; set; }
        internal SourceCodePath? CodeLocation { get; set; }
        
        internal virtual IEnumerable<Element?> GetChildren()
        {
            yield break;
        }

        internal virtual void CreateProxy(Func<Element?, Element?> create)
        {
            
        }
        
        internal abstract SpacePlan Measure(Size availableSpace);
        internal abstract void Draw(Size availableSpace);
    }

    internal readonly struct SourceCodePath(StackFrame frame)
    {
        public readonly string FilePath = frame.GetFileName() ?? string.Empty;
        public readonly int LineNumber = frame.GetFileLineNumber();
        public readonly int ColumnNumber = frame.GetFileColumnNumber();

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
}