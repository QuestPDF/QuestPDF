using System;
using System.Linq;
using System.Runtime.InteropServices;
using QuestPDF.Helpers;

namespace QuestPDF.Skia;

internal static class SkNativeDependencyCompatibilityChecker
{
    private static NativeDependencyCompatibilityChecker Instance { get; } = new()
    {
        ExecuteNativeCode = ExecuteNativeCode
    };
    
    public static void Test()
    {
        Instance.Test();
    }

    private static void ExecuteNativeCode()
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