using System;
using System.Linq;
using System.Runtime.InteropServices;
using QuestPDF.Helpers;

namespace QuestPDF.Skia;

internal static class SkNativeDependencyCompatibilityChecker
{
    public static void CheckIfExceptionIsThrownWhenLoadingNativeDependencies()
    {
        var random = new Random();
            
        var a = random.Next();
        var b = random.Next();
        
        var expected = a + b;
        var returned = API.check_compatibility_by_calculating_sum(a, b);
        
        if (expected != returned)
            throw new Exception();
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int check_compatibility_by_calculating_sum(int a, int b);
    }
}