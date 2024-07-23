using System;
using QuestPDF.Previewer;

namespace QuestPDF.Drawing.Exceptions
{
    public class DocumentLayoutException : Exception
    {
        internal PreviewerCommands.ShowLayoutError PreviewCommand { get; init; }
        
        internal DocumentLayoutException(string message) : base(message)
        {
            
        }
    }
}