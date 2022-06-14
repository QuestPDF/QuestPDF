using System;

namespace QuestPDF.Drawing.Exceptions
{
    public class InitializationException : Exception
    {
        internal InitializationException(string documentType, Exception innerException) : base(CreateMessage(documentType, innerException.Message), innerException)
        {
            
        }

        private static string CreateMessage(string documentType, string innerExceptionMessage)
        {
            var (libraryName, nugetConvention) = GetLibraryName();
            
            return $"Cannot create the {documentType} document using the {libraryName} library. " +
                   $"This exception usually means that, on your operating system where you run the application, {libraryName} requires installing additional dependencies. " +
                   $"Such dependencies are available as additional nuget packages, for example {nugetConvention}.Linux. " +
                   $"Some operating systems may require installing multiple nugets, e.g. MacOS may need both {nugetConvention}.macOS and {nugetConvention}.Linux." +
                   $"Please refer to the {libraryName} documentation for more details. " +
                   $"Also, please consult the inner exception that has been originally thrown by the dependency library.";

            (string GetLibraryName, string nugetConvention) GetLibraryName()
            {
                if (innerExceptionMessage.Contains("libSkiaSharp"))
                    return ("SkiaSharp", "SkiaSharp.NativeAssets.Linux");
                
                if (innerExceptionMessage.Contains("libHarfBuzzSharp"))
                    return ("HarfBuzzSharp", "HarfBuzzSharp.NativeAssets");
                
                // default
                return ("SkiaSharp-related", "*.NativeAssets.Linux");
            }
        }
    }
}