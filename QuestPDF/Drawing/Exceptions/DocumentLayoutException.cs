using System;

namespace QuestPDF.Drawing.Exceptions
{
    public class DocumentLayoutException : Exception
    {
        public string ElementTrace { get; set; }
        
        public DocumentLayoutException()
        {
            
        }

        public DocumentLayoutException(string message) : base(message)
        {
            
        }

        public DocumentLayoutException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}