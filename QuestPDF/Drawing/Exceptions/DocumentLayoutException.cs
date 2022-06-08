using System;

namespace QuestPDF.Drawing.Exceptions
{
    public class DocumentLayoutException : Exception
    {
        public string? ElementTrace { get; }

        internal DocumentLayoutException(string message, string? elementTrace = null) : base(message)
        {
            ElementTrace = elementTrace;
        }
    }
}