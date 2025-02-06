using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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
        Console.WriteLine($"Native files source: {nativeFilesPath}");
        
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
            Console.WriteLine($"Copying native file: {nativeFilePath}; to: {targetPath}");
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
        Console.WriteLine($"Detected platform: {platform}");

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
            Console.WriteLine($"Listing potential native file location: {location}");
        }
        
        foreach (var location in availableLocations)
        {
            if (string.IsNullOrEmpty(location))
                continue;

            var nativeFileSourcePath = Path.Combine(location, "runtimes", platform, "native");

            Console.WriteLine($"Trying access potential native file location: {nativeFileSourcePath}");
            
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
        
        static bool IsLinuxMusl()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return false;
            
            try
            { 
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "ldd",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                using var process = Process.Start(processStartInfo);
                
                if (process == null)
                    return false;
                
                process.Start();
                
                var standardOutputText = process.StandardOutput.ReadToEnd();
                var standardErrorText = process.StandardError.ReadToEnd();
                
                process.WaitForExit();
                
                var outputText = standardOutputText + standardErrorText;
                return outputText.IndexOf("musl", StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch
            {
                return false;
            }
        }
    }

    private static void CopyFileIfNewer(string sourcePath, string targetPath)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException($"Source file not found: {sourcePath}");

        if (!File.Exists(targetPath) || File.GetLastWriteTime(sourcePath) > File.GetLastWriteTime(targetPath))
            File.Copy(sourcePath, targetPath, true);
    }
}