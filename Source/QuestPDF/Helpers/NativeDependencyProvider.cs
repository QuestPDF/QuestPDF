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
        nativeFilePath = Path.GetFullPath(nativeFilePath);
        
#if NET5_0_OR_GREATER
        NativeLibrary.Load(nativeFilePath);
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            const uint LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR = 0x00000100;
            const uint LOAD_LIBRARY_SEARCH_SYSTEM32 = 0x00000800;

            var flags = LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR | LOAD_LIBRARY_SEARCH_SYSTEM32;

            if (LoadLibraryEx(nativeFilePath, IntPtr.Zero, flags) == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
        }
#endif
    }

#if !NET5_0_OR_GREATER
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
                File.Copy(nativeFilePath, targetPath, true);
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
            PathHelpers.GetAssemblyDirectoryOrNull(),
            AppDomain.CurrentDomain.RelativeSearchPath,
            AppDomain.CurrentDomain.BaseDirectory,
            AppContext.BaseDirectory,
            PathHelpers.GetProcessDirectoryOrNull(),
            Environment.CurrentDirectory,
            Directory.GetCurrentDirectory()
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
    
    #endregion
}
