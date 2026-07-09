using System;

namespace QuestPDF.Drawing.Exceptions
{
    public sealed class DocumentComposeException : Exception
    {
        internal DocumentComposeException(string message) : base(message)
        {
            
        }
    }
}