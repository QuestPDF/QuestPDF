using System;
using QuestPDF.Helpers;

namespace QuestPDF.Qpdf;

internal static class QpdfNativeDependencyCompatibilityChecker
{
    private static bool IsCompatibilityChecked = false;
    
    public static void Test()
    {
        if (IsCompatibilityChecked)
            return;
        
        NativeDependencyCompatibilityChecker.Test(ExecuteNativeCode);
        IsCompatibilityChecked = true;

        void ExecuteNativeCode()
        {
            var qpdfVersion = QpdfAPI.GetQpdfVersion();
        
            if (string.IsNullOrEmpty(qpdfVersion))
                throw new Exception();
        }
    }
}