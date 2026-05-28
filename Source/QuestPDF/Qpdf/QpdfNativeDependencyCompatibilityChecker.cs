using System;
using QuestPDF.Helpers;

namespace QuestPDF.Qpdf;

internal static class QpdfNativeDependencyCompatibilityChecker
{
    private const int ExpectedNativeLibraryVersion = 2;
    
    private static NativeDependencyCompatibilityChecker Instance { get; } = new()
    {
        ExecuteNativeCode = ExecuteNativeCode,
        CheckNativeLibraryVersion = CheckNativeLibraryVersion,
        ExceptionHint = GetHint
    };
    
    public static void Test()
    {
        Instance.Test();
    }
    
    private static void ExecuteNativeCode()
    {
        QpdfAPI.GetCompatibilityVersion();
    }
    
    private static bool CheckNativeLibraryVersion()
    {
        try
        {
            return QpdfAPI.GetCompatibilityVersion() == ExpectedNativeLibraryVersion;
        }
        catch
        {
            return false;
        }
    }

    private static string GetHint()
    {
        return $"Please do NOT install the qpdf package.";
    }
}