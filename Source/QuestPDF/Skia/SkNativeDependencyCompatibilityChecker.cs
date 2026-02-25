using System;
using System.Linq;
using System.Runtime.InteropServices;
using QuestPDF.Helpers;

namespace QuestPDF.Skia;

internal static class SkNativeDependencyCompatibilityChecker
{
    private const int ExpectedNativeLibraryVersion = 12;
    
    private static NativeDependencyCompatibilityChecker Instance { get; } = new()
    {
        ExecuteNativeCode = ExecuteNativeCode,
        CheckNativeLibraryVersion = CheckNativeLibraryVersion
    };
    
    public static void Test()
    {
        Instance.Test();
    }

    private static bool CheckNativeLibraryVersion()
    {
        try
        {
            return API.get_questpdf_version() == ExpectedNativeLibraryVersion;
        }
        catch
        {
            return false;
        }
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
        public static extern int get_questpdf_version();
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int check_compatibility_by_calculating_sum(int a, int b);
    }
}