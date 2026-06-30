using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace QuestPDF.Helpers;

internal static class PathHelpers
{
    internal static readonly string ApplicationFilesPath = FindApplicationFilesPath();

    /// <summary>
    /// This method tries to find a path where application resource files (e.g. fonts, images) are stored.
    /// </summary>
    /// <returns></returns>
    private static string FindApplicationFilesPath()
    {
        var candidates = new[]
        {
            AppContext.BaseDirectory, 
            GetAssemblyDirectoryOrNull(), 
            GetProcessDirectoryOrNull(),
            Directory.GetCurrentDirectory(),
        };

        var defaultPath = AppContext.BaseDirectory;

        return candidates
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .FirstOrDefault(x => !IsRoot(x) || ContainsLatoFontFolder(x))
            ?? defaultPath;
        
        // by default, QuestPDF includes the LatoFont folder in application publish artifacts,
        // so presence of this folder is a good indicator that the path is correct.
        static bool ContainsLatoFontFolder(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            
            try
            {
                var latoFontFolderPath = Path.Combine(path, "LatoFont");
                return Directory.Exists(latoFontFolderPath);
            }
            catch
            {
                return false;
            }
        }

        static bool IsRoot(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            
            try
            {
                return new DirectoryInfo(path).Parent == null;
            }
            catch
            {
                return false;
            }
        }
    }

#if NET5_0_OR_GREATER
    [SuppressMessage("SingleFile", "IL3000", Justification = "Code correctly handles the null value when compiled as single file, yet the non-null value on other environments may be useful.")]
#endif
    internal static string? GetAssemblyDirectoryOrNull()
    {
        try
        {
            var location = typeof(PathHelpers).Assembly.Location;
        
            if (string.IsNullOrWhiteSpace(location))
                return null;
            
            return new FileInfo(location).Directory?.FullName;
        }
        catch
        {
            return null;
        }
    }
    
    internal static string? GetProcessDirectoryOrNull()
    {
        try
        {
#if NET6_0_OR_GREATER
            var processPath = Environment.ProcessPath;
#else
            using var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            var processPath = currentProcess.MainModule?.FileName;
#endif
            return string.IsNullOrWhiteSpace(processPath) ? null : Path.GetDirectoryName(processPath);
        }
        catch
        {
            return null;
        }
    }

    internal static IEnumerable<string> EnumerateFilesRecursively(string path)
    {
#if NETSTANDARD2_0
        return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
#else
        var enumerationOptions = new EnumerationOptions
        {
            RecurseSubdirectories = true,
            IgnoreInaccessible = true,
            ReturnSpecialDirectories = false,
            AttributesToSkip = FileAttributes.System | FileAttributes.Hidden | FileAttributes.Offline
        };

        return Directory.EnumerateFiles(path, "*.*", enumerationOptions);
#endif
    }
}