namespace QuestPDF.LayoutTests.TestEngine;

public sealed class LayoutTestException : Exception
{
    internal LayoutTestException(string message) : base(message)
    {
            
    }
    
    internal LayoutTestException(string message, Exception innerException) : base(message, innerException)
    {
            
    }
}