using System;

namespace QuestPDF.Drawing.Exceptions
{
    public sealed class DocumentLayoutException : Exception
    {
        internal DocumentLayoutException(string message) : base(message)
        {
            
        }
    }
}