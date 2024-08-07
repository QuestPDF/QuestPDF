using System;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing.Exceptions
{
    public class DocumentLayoutException : Exception
    {
        internal DocumentLayoutException(string message) : base(message)
        {
            
        }
    }
}