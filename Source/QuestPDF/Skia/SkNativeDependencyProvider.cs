using System;
using System.IO;
using System.Runtime.InteropServices;
using QuestPDF.Drawing.Exceptions;

namespace QuestPDF.Skia;

internal static class SkNativeDependencyProvider
{
    public static void EnsureNativeFileAvailability()
    {
        var nativeFilesPath = GetNativeFileSourcePath();
        
        if (nativeFilesPath == null)
            return;

        foreach (var nativeFilePath in Directory.GetFiles(nativeFilesPath))
        {
            var targetDirectory = new FileInfo(nativeFilePath)
                .Directory
                .Parent // native
                .Parent // platform
                .Parent // runtimes
                .FullName;
            
            var targetPath = Path.Combine(targetDirectory, Path.GetFileName(nativeFilePath));
            CopyFileIfNewer(nativeFilePath, targetPath);
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
    
    static string? GetNativeFileSourcePath()
    {
        var platform = GetRuntimePlatform();

        var availableLocations = new[]
        {
            AppDomain.CurrentDomain.RelativeSearchPath, 
            AppDomain.CurrentDomain.BaseDirectory,
            new FileInfo(typeof(SkNativeDependencyProvider).Assembly.Location).Directory?.FullName
        };
        
        foreach (var location in availableLocations)
        {
            if (string.IsNullOrEmpty(location))
                continue;

            var nativeFileSourcePath = Path.Combine(location, "runtimes", platform, "native");

            if (Directory.Exists(nativeFileSourcePath))
                return nativeFileSourcePath;
        }

        return null;
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

    static void CopyFileIfNewer(string sourcePath, string targetPath)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException($"Source file not found: {sourcePath}");

        if (!File.Exists(targetPath) || File.GetLastWriteTime(sourcePath) > File.GetLastWriteTime(targetPath))
            File.Copy(sourcePath, targetPath, true);
    }
}