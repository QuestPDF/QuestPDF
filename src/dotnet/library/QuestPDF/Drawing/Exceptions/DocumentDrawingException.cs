using System;

namespace QuestPDF.Drawing.Exceptions
{
    public sealed class DocumentDrawingException : Exception
    {
        internal DocumentDrawingException(string message) : base(message)
        {
            
        }
        
        internal DocumentDrawingException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}