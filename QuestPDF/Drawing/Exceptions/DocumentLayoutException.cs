using System;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing.Exceptions
{
    public class DocumentLayoutException : Exception
    {
        internal LayoutErrorTrace? ElementTrace { get; }

        internal DocumentLayoutException(string message, LayoutErrorTrace? elementTrace = null) : base(message)
        {
            ElementTrace = elementTrace;
        }
    }
}