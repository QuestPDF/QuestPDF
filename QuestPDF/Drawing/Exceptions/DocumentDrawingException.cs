using System;

namespace QuestPDF.Drawing.Exceptions
{
    public class DocumentDrawingException : Exception
    {
        public DocumentDrawingException()
        {
            
        }

        public DocumentDrawingException(string message) : base(message)
        {
            
        }

        public DocumentDrawingException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}