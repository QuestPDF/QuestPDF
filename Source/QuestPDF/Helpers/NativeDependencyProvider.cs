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
                "[QuestPDF] The QuestPDF library has encountered an issue while loading one of its dependencies. " +
                "During fallback recovery, QuestPDF could not preload its native dependencies from the 'runtimes/{rid}/native'. " +
                "The standard .NET native library resolution had already failed, and this fallback attempt was not successful. " +
                "Common causes include missing native files, an incompatible runtime identifier, an architecture mismatch, or blocked file access. " +
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
                "[QuestPDF] QuestPDF could not copy its native dependency files to the application directory during fallback recovery. " +
                "The standard .NET native library resolution had already failed, and this fallback attempt was not successful. " +
                "Common causes include insufficient file system permissions, a read-only application directory, or a deployment layout that does not allow files to be copied at runtime. " +
                $"Details: {e.Message}");
        }
    }
    
    private static string? GetNativeFileSourcePath()
    {
        var availableLocations = new[]
        {
            AppDomain.CurrentDomain.RelativeSearchPath,
            AppContext.BaseDirectory, 
            PathHelpers.GetAssemblyDirectoryOrNull(), 
            PathHelpers.GetProcessDirectoryOrNull(),
            Environment.CurrentDirectory
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
