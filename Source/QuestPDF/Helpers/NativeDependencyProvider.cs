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
        "win-arm64",
        "linux-x64",
        "linux-arm64",
        "linux-musl-x64",
        "osx-x64",
        "osx-arm64"
    };
    
    public static void EnsureNativeFileAvailability()
    {
        try
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
        catch (Exception e)
        {
            Trace.TraceWarning(
                "QuestPDF was unable to copy its native dependency files to the application directory. " +
                "This is a best-effort fallback and is not necessarily a problem; native libraries may still load correctly through the standard .NET runtime mechanism. " +
                "However, if document generation subsequently fails with a native dependency error, the most likely causes are insufficient file system permissions or a read-only application directory. " +
                $"Details: {e.Message}");
        }
    }
    
    public static bool IsCurrentRuntimeSupported()
    {
        return SupportedPlatforms.Contains(RuntimePlatform.Value);
    }
    
    static string? GetNativeFileSourcePath()
    {
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

            var nativeFileSourcePath = Path.Combine(location, "runtimes", RuntimePlatform.Value, "native");

            if (Directory.Exists(nativeFileSourcePath))
                return nativeFileSourcePath;
        }

        return null;
    }

    public static readonly Lazy<string> RuntimePlatform = new(() =>
    {
        return $"{GetSystemIdentifier()}-{GetProcessArchitecture()}";

        static string GetSystemIdentifier()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return "win";
                
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return IsLinuxMusl.Value ? "linux-musl" : "linux";
                
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return "osx";

            return "other";
        }

        static string GetProcessArchitecture()
        {
            return RuntimeInformation.ProcessArchitecture.ToString().ToLower();
        }
    });
    
    private static readonly Lazy<string?> LddCommandOutput = new(() =>
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
    });

    private static readonly Lazy<bool> IsLinuxMusl = new(() =>
    {
        var lddCommandOutput = LddCommandOutput.Value ?? string.Empty;
        return lddCommandOutput.IndexOf("musl", StringComparison.InvariantCultureIgnoreCase) >= 0;
    });

    public static readonly Lazy<Version?> GlibcVersion = new(() =>
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return null;
        
        // strategy 1: use gnu_get_libc_version
        try
        {
            var versionPtr = gnu_get_libc_version();
            var versionText = Marshal.PtrToStringAnsi(versionPtr);
            return Version.Parse(versionText);
        }
        catch
        {
            // ignore
        }
        
        // strategy 2: use ldd command
        var lddCommandOutput = LddCommandOutput.Value ?? string.Empty;
        
        if (string.IsNullOrEmpty(lddCommandOutput))
            return null;
        
        var match = Regex.Match(lddCommandOutput, @"ldd \(.+\) (?<version>\d+\.\d+)");
    
        if (!match.Success)
            return null;

        var versionGroup = match.Groups["version"];
        
        if (!versionGroup.Success)
            return null;
    
        return Version.TryParse(versionGroup.Value, out var parsedVersion) ? parsedVersion : null;
    });
    
    [DllImport("libc", EntryPoint = "gnu_get_libc_version")]
    private static extern IntPtr gnu_get_libc_version();

    private static void CopyFileIfNewer(string sourcePath, string targetPath)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException($"Source file not found: {sourcePath}");

        if (!File.Exists(targetPath) || File.GetLastWriteTime(sourcePath) > File.GetLastWriteTime(targetPath))
            File.Copy(sourcePath, targetPath, true);
    }
}