using System;
using System.IO;
using System.Runtime.InteropServices;
using QuestPDF.Drawing.Exceptions;

namespace QuestPDF.Skia;

internal static class SkNativeDependencyProvider
{
    private static string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

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
            GetRuntimePlatform();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    static string GetNativeFileSourcePath()
    {
        var platform = GetRuntimePlatform();
        var nativeFileSourcePath = Path.Combine(_baseDirectory, "runtimes", platform, "native");

        if (Directory.Exists(nativeFileSourcePath))
            return nativeFileSourcePath;

        var applicationDirectory = AppDomain.CurrentDomain.RelativeSearchPath;

        if (!string.IsNullOrEmpty(applicationDirectory))
            nativeFileSourcePath = Path.Combine(applicationDirectory, "runtimes", platform, "native");

        if (!Directory.Exists(nativeFileSourcePath))
            throw new DirectoryNotFoundException($"Native files not found in {nativeFileSourcePath}");

        _baseDirectory = applicationDirectory!;
        return nativeFileSourcePath;
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

        throw new InitializationException("Your runtime is currently not supported by QuestPDF.");
    }
        
    static string GetNativeFileRuntimePath(string fileName)
    {
        return Path.Combine(_baseDirectory, fileName);
    }

    static void CopyFileIfNewer(string sourcePath, string targetPath)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException($"Source file not found: {sourcePath}");

        if (!File.Exists(targetPath) || File.GetLastWriteTime(sourcePath) > File.GetLastWriteTime(targetPath))
            File.Copy(sourcePath, targetPath, true);
    }
}