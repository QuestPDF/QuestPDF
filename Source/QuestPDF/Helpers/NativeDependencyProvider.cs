using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace QuestPDF.Helpers;

internal static class NativeDependencyProvider
{
    #region Native Library Preloading

    /// <summary>
    /// Loads the native dependencies directly from the "runtimes/{rid}/native" folder by absolute path.
    /// Unlike <see cref="EnsureNativeFileAvailability"/>, this requires no write access to the application
    /// directory, which makes it robust in read-only or restricted deployments (for example, IIS hosted
    /// from Program Files). This is primarily relevant on .NET Framework, where the runtime does not probe
    /// the "runtimes/{rid}/native" folder automatically.
    /// </summary>
    public static void TryPreloadNativeDependencies()
    {
        var nativeFilesPath = GetNativeFileSourcePath();

        if (nativeFilesPath == null)
            return;

        foreach (var baseName in new[] { "QuestPdfSkia", "qpdf" })
        {
            var nativeFilePath = Path.Combine(nativeFilesPath, GetNativeLibraryFileName(baseName));

            if (File.Exists(nativeFilePath))
                LoadNativeLibrary(nativeFilePath);
        }
    }

    private static string GetNativeLibraryFileName(string baseName)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return $"{baseName}.dll";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return $"lib{baseName}.dylib";

        return $"lib{baseName}.so";
    }

    private static void LoadNativeLibrary(string nativeFilePath)
    {
#if NET6_0_OR_GREATER
        NativeLibrary.Load(nativeFilePath);
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            const uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;

            if (LoadLibraryEx(nativeFilePath, IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH) == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }
        else
        {
            const int RTLD_NOW = 0x0002;
            const int RTLD_GLOBAL = 0x0100;

            if (dlopen(nativeFilePath, RTLD_NOW | RTLD_GLOBAL) == IntPtr.Zero)
                throw new InvalidOperationException($"Failed to load native library '{nativeFilePath}'.");
        }
#endif
    }

#if !NET6_0_OR_GREATER
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

    [DllImport("libdl", EntryPoint = "dlopen")]
    private static extern IntPtr dlopen(string fileName, int flags);
#endif

    #endregion
    
    #region Native File Copying

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

            var nativeFileSourcePath = Path.Combine(location, "runtimes", NativeRuntimeDetection.RuntimePlatform.Value, "native");

            if (Directory.Exists(nativeFileSourcePath))
                return nativeFileSourcePath;
        }

        return null;
    }
    
    private static void CopyFileIfNewer(string sourcePath, string targetPath)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException($"Source file not found: {sourcePath}");

        if (!File.Exists(targetPath) || File.GetLastWriteTime(sourcePath) > File.GetLastWriteTime(targetPath))
            File.Copy(sourcePath, targetPath, true);
    }
    
    #endregion
}