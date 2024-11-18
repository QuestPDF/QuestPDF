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
        
        const string openSslHint = "Please also ensure that the OpenSSL library is installed on your system with version at least 3.0.0.";
        
        var command = platform switch
        {
            "linux-x64" or "linux-arm64" => "apt install openssl-bin gnutls-bin libjpeg-dev",
            "linux-musl-x64" => "apk add openssl gnutls libjpeg-turbo",
            _ => throw new NotSupportedException()
        };
        
        return $"Installing additional dependencies may help. Likely command: '{command}'. {openSslHint}";
    }
}