using System;
using QuestPDF.Helpers;

namespace QuestPDF.Qpdf;

internal static class QpdfNativeDependencyCompatibilityChecker
{
    private static NativeDependencyCompatibilityChecker Instance { get; } = new()
    {
        ExecuteNativeCode = ExecuteNativeCode,
        ExceptionHint = GetHint
    };
    
    public static void Test()
    {
        Instance.Test();
    }
    
    private static void ExecuteNativeCode()
    {
        var qpdfVersion = QpdfAPI.GetQpdfVersion();
        
        if (string.IsNullOrEmpty(qpdfVersion))
            throw new Exception();
    }

    private static string GetHint()
    {
        var platform = NativeDependencyProvider.GetRuntimePlatform();

        if (platform != "linux-musl-x64")
            return string.Empty;
        
        return $"Installing additional dependencies may help. Please try the following command: 'apk add libjpeg-turbo'. Do NOT install the qpdf package.";
    }
}