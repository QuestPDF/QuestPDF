using System;
using System.Runtime.InteropServices;
using QuestPDF.Drawing.Exceptions;

namespace QuestPDF.Skia;

public static class SkNativeDependencyCompatibilityChecker
{
    private static bool IsCompatibilityChecked = false;
        
    public static void Test()
    {
        if (IsCompatibilityChecked)
            return;
            
        // test with dotnet-based mechanism where native files are provided
        // in the "runtimes/{rid}/native" folder on Core, or by the targets file on .NET Framework
        var innerException = CheckIfExceptionIsThrownWhenLoadingNativeDependencies();

        if (innerException == null)
        {
            IsCompatibilityChecked = true;
            return;
        }
        
        if (SkNativeDependencyProvider.IsCurrentPlatformSupported())
            throw new InitializationException($"Your runtime is currently not supported by QuestPDF.");
        
        // detect platform, copy appropriate native files and test compatibility again
        SkNativeDependencyProvider.EnsureNativeFileAvailability();
        
        innerException = CheckIfExceptionIsThrownWhenLoadingNativeDependencies();

        if (innerException == null)
        {
            IsCompatibilityChecked = true;
            return;
        }

        var initializationExceptionMessage = $"The QuestPDF library has encountered an issue while loading one of its dependencies.";
        
        throw new Exception(initializationExceptionMessage, innerException);
    }
    
    private static Exception? CheckIfExceptionIsThrownWhenLoadingNativeDependencies()
    {
        try
        {
            var random = new Random();
            
            var a = random.Next();
            var b = random.Next();
        
            var expected = a + b;
            var returned = API.check_compatibility_by_calculating_sum(a, b);
        
            if (expected != returned)
                throw new Exception();

            return null;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern int check_compatibility_by_calculating_sum(int a, int b);
    }
}