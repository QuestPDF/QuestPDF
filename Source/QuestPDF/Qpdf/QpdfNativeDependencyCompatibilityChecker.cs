using System;
using QuestPDF.Helpers;

namespace QuestPDF.Qpdf;

internal static class QpdfNativeDependencyCompatibilityChecker
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
        var qpdfVersion = QpdfAPI.GetQpdfVersion();
        
        if (string.IsNullOrEmpty(qpdfVersion))
            throw new Exception();
    }
}