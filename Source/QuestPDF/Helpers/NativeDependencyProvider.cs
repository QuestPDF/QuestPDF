using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace QuestPDF.Helpers;

internal static class NativeDependencyProvider
{
    #region Native Library Preloading

    public static void TryPreloadNativeDependencies()
    {
        try
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
        catch (Exception e)
        {
            Trace.TraceWarning(
                "QuestPDF was unable to preload its native dependencies from the 'runtimes/{rid}/native' folder. " +
                "This operation runs only after the standard .NET native library resolution has already failed, as part of QuestPDF's recovery process. " +
                "The most likely causes are a missing or incompatible native binary, or a runtime/architecture mismatch. " +
                $"Details: {e.Message}");
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
#endif
    }

#if !NET6_0_OR_GREATER
    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);
#endif

    #endregion
    
    #region Native File Copying

    public static void TryCopyNativeDependenciesToApplicationDirectory()
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
                "This operation runs only after the standard .NET native library resolution has already failed, as part of QuestPDF's recovery process. " +
                "The most likely causes are insufficient file system permissions or a read-only application directory. " +
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
            GetAssemblyDirectoryOrNull()
        };
        
        foreach (var location in availableLocations.Distinct())
        {
            if (string.IsNullOrEmpty(location))
                continue;

            var nativeFileSourcePath = Path.Combine(location, "runtimes", NativeRuntimeDetection.RuntimePlatform.Value, "native");

            if (Directory.Exists(nativeFileSourcePath))
                return nativeFileSourcePath;
        }

        return null;
    }
    
    private static string? GetAssemblyDirectoryOrNull()
    {
        try
        {
            var location = typeof(NativeDependencyProvider).Assembly.Location;
            return new FileInfo(location).Directory?.FullName;
        }
        catch
        {
            return null;
        }
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