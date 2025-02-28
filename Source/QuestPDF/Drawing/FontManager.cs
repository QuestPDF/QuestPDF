using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;
using QuestPDF.Skia.Text;

namespace QuestPDF.Drawing
{
    /// <summary>
    /// <para>By default, the library searches all fonts available in the runtime environment.</para>
    /// <para>This may work well on the development environment but may fail in the cloud where fonts are usually not installed.</para>
    /// <para>It is safest deploy font files along with the application and then register them using this class.</para>
    /// </summary>
    public static class FontManager
    {
        internal static SkTypefaceProvider TypefaceProvider { get; } = new();
        internal static SkFontManager CurrentFontManager => Settings.UseEnvironmentFonts ? SkFontManager.Global : SkFontManager.Local;

        static FontManager()
        {
            SkNativeDependencyCompatibilityChecker.Test();
            RegisterLibraryDefaultFonts();
        }
        
        [Obsolete("Since version 2022.8 this method has been renamed. Please use the RegisterFontWithCustomName method.")]
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
            TypefaceProvider.AddTypefaceFromData(fontData);
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
                        return Directory
                            .EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
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