using System.Runtime.InteropServices;
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
        return "QuestPDF bundles its own qpdf native library. " +
               "If a different, system-wide qpdf is installed, it may be loaded instead of the bundled one and cause a version conflict. " +
               "If present, please remove the system 'qpdf' package.";
    }
}