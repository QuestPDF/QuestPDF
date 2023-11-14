using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using QuestPDF.Drawing.Exceptions;

namespace QuestPDF.Helpers
{
    internal static class NativeDependencyCompatibilityChecker
    {
        private static bool IsCompatibilityChecked = false;
        
        public static void Test()
        {
            if (IsCompatibilityChecked)
                return;
            
            var innerException = CheckIfExceptionIsThrownWhenLoadingNativeDependencies();

            if (innerException == null)
            {
                IsCompatibilityChecked = true;
                return;
            }

            var newLine = Environment.NewLine;
            var paragraph = newLine + newLine;

            var initializationExceptionMessage = 
                $"The QuestPDF library has encountered an issue while loading one of its dependencies. " +
                $"This type of error often occurs when the current runtime is missing necessary NuGet packages. {paragraph}" +
                $"Please ensure the following NuGet packages are added to your primary/startup project and has matching versions:";

            foreach (var nuget in GetRecommendedNugetDependencies())
                initializationExceptionMessage += $"{newLine}- {nuget}";

            initializationExceptionMessage += $"{paragraph}For a detailed understanding, please examine the inner exception and consult the official SkiaSharp library documentation.";
            
            throw new InitializationException(initializationExceptionMessage, innerException);
        }

        private static Exception? CheckIfExceptionIsThrownWhenLoadingNativeDependencies()
        {
            try
            {
                // accessing any SkiaSharp object triggers loading of SkiaSharp-related DLL dependency
                using var typeface = new SkiaSharp.SKPaint();

                // accessing any HarfBuzzSharp object triggers loading of HarfBuzz-related DLL dependency
                using var shaper = new HarfBuzzSharp.Buffer();

                // everything loads and works correctly
                return null;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }
        
        private static IEnumerable<string> GetRecommendedNugetDependencies()
        {
            const string skiaSharp = "SkiaSharp.NativeAssets";
            const string harfBuzzSharp = "HarfBuzzSharp.NativeAssets";
            
            #if NET5_0_OR_GREATER
            
            if (OperatingSystem.IsMacOS())
            {
                yield return $"{skiaSharp}.macOS";
                yield return $"{harfBuzzSharp}.macOS";
            }
            else if (OperatingSystem.IsMacCatalyst())
            {
                yield return $"{skiaSharp}.MacCatalyst";
                yield return $"{harfBuzzSharp}.MacCatalyst";
            }
            else if (OperatingSystem.IsIOS())
            {
                yield return $"{skiaSharp}.iOS";
                yield return $"{harfBuzzSharp}.iOS";
            }
            else if (OperatingSystem.IsWatchOS())
            {
                yield return $"{skiaSharp}.watchOS";
                yield return $"{harfBuzzSharp}.watchOS";
            }
            else if (OperatingSystem.IsTvOS())
            {
                yield return $"{skiaSharp}.tvOS";
                yield return $"{harfBuzzSharp}.tvOS";
            }
            else if (OperatingSystem.IsAndroid())
            {
                yield return $"{skiaSharp}.Android";
                yield return $"{harfBuzzSharp}.Android";
            }
            else if (OperatingSystem.IsBrowser())
            {
                yield return $"{skiaSharp}.WebAssembly";
                yield return $"{harfBuzzSharp}.WebAssembly";
            }
            else if (OperatingSystem.IsLinux())
            {
                yield return $"{skiaSharp}.Linux.NoDependencies";
                yield return $"{harfBuzzSharp}.Linux";
            }
            else if (OperatingSystem.IsWindows())
            {
                yield return $"{skiaSharp}.Win32";
                yield return $"{harfBuzzSharp}.Win32";
            }
            
            #else
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                yield return $"{skiaSharp}.Linux.NoDependencies";
                yield return $"{harfBuzzSharp}.Linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                yield return $"{skiaSharp}.macOS";
                yield return $"{harfBuzzSharp}.macOS";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                yield return $"{skiaSharp}.Win32";
                yield return $"{harfBuzzSharp}.Win32";
            }
            
            #endif
        }
    }
}