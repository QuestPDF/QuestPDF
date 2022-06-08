using System;

namespace QuestPDF.Drawing.Exceptions
{
    public class DocumentDrawingException : Exception
    {
        internal DocumentDrawingException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}