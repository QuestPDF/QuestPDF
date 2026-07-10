using System;
using System.Linq;
using System.Runtime.InteropServices;
using QuestPDF.Helpers;

namespace QuestPDF.Skia;

internal static class SkNativeDependencyCompatibilityChecker
{
    private const int ExpectedNativeLibraryVersion = 16;
    
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
            return API.questpdf_skia_get_compatibility_version() == ExpectedNativeLibraryVersion;
        }
        catch
        {
            return false;
        }
    }
    
    private static void ExecuteNativeCode()
    {
        API.questpdf_skia_get_compatibility_version();
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int questpdf_skia_get_compatibility_version();
    }
}