using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

public class SkNativeDependencyCompatibilityChecker
{
    private static bool IsCompatibilityChecked = false;
        
    public static void Test()
    {
        if (IsCompatibilityChecked)
            return;
            
        var innerException = CheckIfExceptionIsThrownWhenLoadingNativeDependencies();

        if (innerException == null)
        {
            IsCompatibilityChecked = true;
            return;
        }

        // TODO: improve error message
        var initializationExceptionMessage =
            $"The QuestPDF library has encountered an issue while loading one of its dependencies.";
        
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