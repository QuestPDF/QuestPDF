using System;

namespace QuestPDF.Drawing.Exceptions
{
    public class InitializationException : Exception
    {
        internal InitializationException(string documentType, Exception innerException) : base(CreateMessage(documentType), innerException)
        {
            
        }

        private static string CreateMessage(string documentType)
        {
            return $"Cannot create the {documentType} document using the SkiaSharp library. " +
                   $"This exception usually means that, on your operating system where you run the application, SkiaSharp requires installing additional dependencies. " +
                   $"Such dependencies are available as additional nuget packages, for example SkiaSharp.NativeAssets.Linux. " +
                   $"Please refer to the SkiaSharp documentation for more details.";
        }
    }
}