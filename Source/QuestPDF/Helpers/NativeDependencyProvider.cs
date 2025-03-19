using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace QuestPDF.Helpers;

internal static class NativeDependencyProvider
{
    public static readonly string[] SupportedPlatforms =
    {
        "win-x86",
        "win-x64",
        "linux-x64",
        "linux-arm64",
        "linux-musl-x64",
        "osx-x64",
        "osx-arm64"
    };
    
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
        var currentRuntime = GetRuntimePlatform();
        return SupportedPlatforms.Contains(currentRuntime);
    }
    
    static string? GetNativeFileSourcePath()
    {
        var platform = GetRuntimePlatform();

        var availableLocations = new[]
        {
            AppDomain.CurrentDomain.RelativeSearchPath, 
            AppDomain.CurrentDomain.BaseDirectory,
            Environment.CurrentDirectory,
            AppContext.BaseDirectory,
            Directory.GetCurrentDirectory(),
            new FileInfo(typeof(NativeDependencyProvider).Assembly.Location).Directory?.FullName
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
        
    public static string GetRuntimePlatform()
    {
#if NET6_0_OR_GREATER
        if (RuntimeInformation.ProcessArchitecture == Architecture.Wasm)
            return "browser-wasm";
#endif
        
        return $"{GetSystemIdentifier()}-{GetProcessArchitecture()}";

        static string GetSystemIdentifier()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win";
                
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return IsLinuxMusl() ? "linux-musl" : "linux";
                
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";

            return "other";
        }

        static string GetProcessArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture.ToString().ToLower();
        }
    }
    
    private static string? ExecuteLddCommand()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return null;
        
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ldd",
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
                
            using var process = Process.Start(processStartInfo);
                
            if (process == null)
                return null;
                
            var standardOutputText = process.StandardOutput.ReadToEnd();
            var standardErrorText = process.StandardError.ReadToEnd();
                
            process.WaitForExit();
                
            return standardOutputText + standardErrorText;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsLinuxMusl()
    {
        var lddCommandOutput = ExecuteLddCommand() ?? string.Empty;
        var containsMuslText = lddCommandOutput.IndexOf("musl", StringComparison.InvariantCultureIgnoreCase) >= 0;
        return containsMuslText;
    }

    public static Version? GetGlibcVersion()
    {
        var lddCommandOutput = ExecuteLddCommand() ?? string.Empty;
        var match = Regex.Match(lddCommandOutput, @"ldd \(.+\) (?<version>[1-9]\.[0-9]{2})");
    
        if (!match.Success)
            return null;

        var versionGroup = match.Groups["version"];
        
        if (!versionGroup.Success)
            return null;
    
        return Version.TryParse(versionGroup.Value, out var parsedVersion) ? parsedVersion : null;
    }

    private static void CopyFileIfNewer(string sourcePath, string targetPath)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException($"Source file not found: {sourcePath}");

        if (!File.Exists(targetPath) || File.GetLastWriteTime(sourcePath) > File.GetLastWriteTime(targetPath))
            File.Copy(sourcePath, targetPath, true);
    }
}