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
        
        if (!platform.StartsWith("linux"))
            return string.Empty;
        
        var command = platform switch
        {
            "linux-x64" or "linux-arm64" => "apt install openssl libjpeg-turbo8",
            "linux-musl-x64" => "apk add openssl libjpeg-turbo",
            _ => throw new NotSupportedException()
        };
        
        const string openSslHint = "Please also ensure that the OpenSSL library is installed on your system with version at least 3.0.0.";
        const string qpdfHint = "Do NOT install the qpdf package.";
        
        return $"Installing additional dependencies may help. Please try the following command: '{command}'. {openSslHint} {qpdfHint}";
    }
}