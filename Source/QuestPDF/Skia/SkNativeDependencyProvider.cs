using System;
using System.IO;
using System.Runtime.InteropServices;
using QuestPDF.Drawing.Exceptions;

namespace QuestPDF.Skia;

internal static class SkNativeDependencyProvider
{
    public static void EnsureNativeFileAvailability()
    {
        var nativeFiles = Directory.GetFiles(GetNativeFileSourcePath());

        foreach (var nativeFileSourcePath in nativeFiles)
        {
            var nativeFileName = Path.GetFileName(nativeFileSourcePath);
            var runtimePath = GetNativeFileRuntimePath(nativeFileName);

            CopyFileIfNewer(nativeFileSourcePath, runtimePath);
        }
    }
    
    public static bool IsCurrentPlatformSupported()
    {
        try
        {
            EnsureNativeFileAvailability();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    static string GetNativeFileSourcePath()
    {
        var applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var platform = GetRuntimePlatform();
        return Path.Combine(applicationDirectory, "runtimes", platform, "native");
    }
        
    static string GetRuntimePlatform()
    {
        if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win-x64";
                
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return "linux-x64";
                
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx-x64";
        }
            
        if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx-arm64";
        }

        throw new InitializationException("Your runtime is currently not supported by QuestPDF");
    }
        
    static string GetNativeFileRuntimePath(string fileName)
    {
        var applicationDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(applicationDirectory, fileName);
    }

    static void CopyFileIfNewer(string sourcePath, string targetPath)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException($"Source file not found: {sourcePath}");

        if (!File.Exists(targetPath) || File.GetLastWriteTime(sourcePath) > File.GetLastWriteTime(targetPath))
            File.Copy(sourcePath, targetPath, true);
    }
}