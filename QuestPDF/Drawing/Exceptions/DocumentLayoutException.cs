using System;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing.Exceptions
{
    public class DocumentLayoutException : Exception
    {
        internal LayoutRenderingTrace? ElementTrace { get; }

        internal DocumentLayoutException(string message, LayoutRenderingTrace? elementTrace = null) : base(message)
        {
            ElementTrace = elementTrace;
        }
    }
}