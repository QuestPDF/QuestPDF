using System;
using QuestPDF.Qpdf;

namespace QuestPDF.Skia;

internal static class QpdfNativeDependencyCompatibilityChecker
{
    public static void CheckIfExceptionIsThrownWhenLoadingNativeDependencies()
    {
        QpdfAPI.Initialize();
        
        var qpdfVersion = QpdfAPI.GetQpdfVersion();
        
        if (string.IsNullOrEmpty(qpdfVersion))
            throw new Exception();
    }
}