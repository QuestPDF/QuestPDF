using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing
{
    /// <summary>
    /// <para>By default, the library uses the fonts installed on the system where the application is running (see <see cref="Settings.UseSystemFonts"/>).</para>
    /// <para>This may work well on the development environment but may fail in the cloud where fonts are usually not installed.</para>
    /// <para>It is safest deploy font files along with the application. QuestPDF automtically scans all fonts deployed along with the application. Optionally, you can register additional fonts this class.</para>
    /// </summary>
    public static class FontManager
    {
        internal static SkTypefaceProvider TypefaceProvider { get; } = new();
        internal static SkFontManager? SystemFontManager => Settings.UseSystemFonts ? SkFontManager.System : null;

        static FontManager()
        {
            SkNativeDependencyCompatibilityChecker.Test();
            RegisterLibraryDefaultFonts();
        }
        
        [Obsolete("Since version 2022.8 this method has been renamed. Please use the RegisterFontWithCustomName method.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [ExcludeFromCodeCoverage]
        public static void RegisterFontType(string fontName, Stream stream)
        {
            RegisterFontWithCustomName(fontName, stream);
        }
        
        /// <summary>
        /// Registers a TrueType font from a stream under the provided custom <paramref name="fontName"/>.
        /// Refer to this font by using the same name as a font family in the <see cref="TextStyle"/> API later on.
        /// <a href="https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration">Learn more</a>
        /// </summary>
        public static void RegisterFontWithCustomName(string fontName, Stream stream)
        {
            using var fontData = SkData.FromStream(stream);
            TypefaceProvider.AddTypefaceFromData(fontData, fontName);
        }

        /// <summary>
        /// Registers a TrueType font from a stream. The font family name and all related attributes are detected automatically.
        /// <a href="https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration">Learn more</a>
        /// </summary>
        public static void RegisterFont(Stream stream)
        {
            using var fontData = SkData.FromStream(stream);
            TypefaceProvider.AddTypefaceFromData(fontData);
        }
        
        /// <summary>
        /// Registers a TrueType font from an embedded resource. The font family name and all related attributes are detected automatically.
        /// <a href="https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration">Learn more</a>
        /// </summary>
        /// <param name="pathName">Path to the embedded resource (the case-sensitive name of the manifest resource being requested).</param>
        public static void RegisterFontFromEmbeddedResource(string pathName)
        {
            using var stream = Assembly.GetCallingAssembly().GetManifestResourceStream(pathName);

            if (stream == null)
                throw new ArgumentException($"Cannot load font file from an embedded resource. Please make sure that the resource is available or the path is correct: {pathName}");
            
            RegisterFont(stream);
        }
        
        /// <summary>
        /// Returns information about the fonts registered in the library: fonts discovered automatically in the <see cref="Settings.FontDiscoveryPaths"/> directories,
        /// and fonts registered manually with the <see cref="RegisterFont"/>, <see cref="RegisterFontWithCustomName"/> and <see cref="RegisterFontFromEmbeddedResource"/> methods.
        /// These fonts are always available to the library, regardless of the runtime environment.
        /// </summary>
        /// <remarks>
        /// <para>Each entry describes a single typeface (font face), e.g. the regular and bold faces of one family are listed separately.</para>
        /// <para>A typeface registered under several names (e.g. a custom name and its own family name) is listed once per name.</para>
        /// </remarks>
        public static IReadOnlyCollection<FontInfo> GetRegisteredFonts()
        {
            return TypefaceProvider.GetTypefaces();
        }
        
        /// <summary>
        /// Returns information about the fonts installed on the system where the application is running, as visible to the library.
        /// System fonts are used only when <see cref="Settings.UseSystemFonts"/> is enabled.
        /// </summary>
        /// <remarks>
        /// <para>Each entry describes a single typeface (font face), e.g. the regular and bold faces of one family are listed separately.</para>
        /// <para>The result is not cached. Enumerating system fonts may take a moment on systems with many fonts installed.</para>
        /// </remarks>
        public static IReadOnlyCollection<FontInfo> GetSystemFonts()
        {
            return SkFontManager.System.GetTypefaces();
        }
        
        private static void RegisterLibraryDefaultFonts()
        {
            var fontFilePaths = SearchFontFiles();
            
            foreach (var fileName in fontFilePaths)
            {
                try
                {
                    using var fontFileStream = File.OpenRead(fileName);
                    RegisterFont(fontFileStream);
                }
                catch
                {
                    
                }
            }

            ICollection<string> SearchFontFiles()
            {
                const int maxFilesToScan = 100_000;
                
                var applicationFiles = Settings
                    .FontDiscoveryPaths
                    .Where(Directory.Exists)
                    .Select(TryEnumerateFiles)
                    .SelectMany(file => file)
                    .Take(maxFilesToScan)
                    .ToList();
                
                if (applicationFiles.Count == maxFilesToScan)
                    throw new InvalidOperationException($"The library has reached the limit of {maxFilesToScan} files to scan for font files. Please adjust the {nameof(Settings.FontDiscoveryPaths)} collection to include only the necessary directories. The reason of this exception is to prevent scanning too many files and avoid performance issues on the application startup.");
                
                var supportedFontExtensions = new[] { ".ttf", ".otf", ".ttc", ".pfb" };
                
                return applicationFiles
                    .Where(x => supportedFontExtensions.Contains(Path.GetExtension(x).ToLowerInvariant()))
                    .ToList();
                
                ICollection<string> TryEnumerateFiles(string path)
                {
                    try
                    {
                        return PathHelpers
                            .EnumerateFilesRecursively(path)
                            .Take(maxFilesToScan)
                            .ToArray();
                    }
                    catch
                    {
                        return Array.Empty<string>();
                    }
                }
            }
        }
    }
}