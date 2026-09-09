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
    /// <para>By default, the library uses only the fonts registered with this class: font files discovered automatically in the <see cref="Settings.FontDiscoveryPath"/> directory (by default, the application directory) and fonts registered manually with the methods below.</para>
    /// <para>Fonts installed on the system where the application is running are ignored unless <see cref="Settings.UseSystemFonts"/> is enabled. This keeps the output independent of the runtime environment, e.g. the cloud or containers where fonts are usually not installed.</para>
    /// <para>It is safest to deploy font files along with the application. Optionally, use this class to register additional fonts, e.g. from a stream or an embedded resource.</para>
    /// </summary>
    public static class FontManager
    {
        internal static SkTypefaceProvider TypefaceProvider { get; } = new();
        internal static SkFontManager? SystemFontManager => Settings.UseSystemFonts ? SkFontManager.System : null;

        static FontManager()
        {
            SkNativeDependencyCompatibilityChecker.Test();
            RegisterFontsFromDiscoveryPath();
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
            RegisterTypeface(fontData, fontName);
        }

        /// <summary>
        /// Registers a TrueType font from a stream. The font family name and all related attributes are detected automatically.
        /// <a href="https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration">Learn more</a>
        /// </summary>
        public static void RegisterFont(Stream stream)
        {
            using var fontData = SkData.FromStream(stream);
            RegisterTypeface(fontData);
        }
        
        private static void RegisterTypeface(SkData fontData, string? alias = null)
        {
            lock (FontFamilyNamesLock)
            {
                TypefaceProvider.AddTypefaceFromData(fontData, alias);
                RegisteredFontFamilyNamesCache = null;
            }
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
        /// Registers a TrueType font from a file.
        /// <a href="https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration">Learn more</a>
        /// </summary>
        /// <param name="path">Path to the font file (.ttf, .otf, .ttc or .pfb). Absolute paths are used as-is. Relative paths are resolved first against the current working directory, and then against the application directory.</param>
        public static void RegisterFontFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            
            path = PathHelpers.ResolveResourceFilePath(path);
            
            using var fontData = SkData.FromFile(path);
            RegisterTypeface(fontData);
        }
        
        private static void TryRegisterFontFromFile(string path)
        {
            try
            {
                RegisterFontFromFile(path);
            }
            catch
            {
                // files that cannot be read or are not valid font files are skipped
            }
        }
        
        /// <summary>
        /// Registers all font files (.ttf, .otf, .ttc and .pfb) found in the provided directory and its subdirectories.
        /// The font family names and all related attributes are detected automatically.
        /// </summary>
        /// <remarks>
        /// <para>Font files in the <see cref="Settings.FontDiscoveryPath"/> directory (by default, the application directory) are registered automatically. Use this method to register fonts from additional directories.</para>
        /// <para>Files that are not valid font files are skipped.</para>
        /// </remarks>
        /// <param name="path">Path to the directory containing font files.</param>
        public static void RegisterFontsFromDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            
            if (!Directory.Exists(path))
                throw new ArgumentException($"Cannot register fonts from a directory. Please make sure that the directory exists or the path is correct: {path}");

            var fontFiles = PathHelpers
                .EnumerateFilesRecursively(path)
                .FilterFontFiles();
                
            foreach (var fontFile in fontFiles)
                TryRegisterFontFromFile(fontFile);
        }
        
        /// <summary>
        /// Returns information about the fonts registered in the library: fonts discovered automatically in the <see cref="Settings.FontDiscoveryPath"/> directory,
        /// and fonts registered manually with the <see cref="RegisterFont"/>, <see cref="RegisterFontWithCustomName"/>, <see cref="RegisterFontFromEmbeddedResource"/> and <see cref="RegisterFontsFromDirectory"/> methods.
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
        /// System fonts are used only when <see cref="Settings.UseSystemFonts"/> is enabled (it is disabled by default).
        /// </summary>
        /// <remarks>
        /// <para>Each entry describes a single typeface (font face), e.g. the regular and bold faces of one family are listed separately.</para>
        /// <para>The result is not cached. Enumerating system fonts may take a moment on systems with many fonts installed.</para>
        /// </remarks>
        public static IReadOnlyCollection<FontInfo> GetSystemFonts()
        {
            return SkFontManager.System.GetTypefaces();
        }
        
        #region Checking Font Family Availability
        
        private static readonly object FontFamilyNamesLock = new();
        private static volatile HashSet<string>? RegisteredFontFamilyNamesCache;
        private static volatile HashSet<string>? SystemFontFamilyNamesCache;
        
        internal static bool IsFontFamilyAvailable(string fontFamily)
        {
            if (GetRegisteredFontFamilyNames().Contains(fontFamily))
                return true;
            
            return Settings.UseSystemFonts && GetSystemFontFamilyNames().Contains(fontFamily);
        }
        
        private static HashSet<string> GetRegisteredFontFamilyNames()
        {
            var names = RegisteredFontFamilyNamesCache;
            
            if (names != null)
                return names;
            
            lock (FontFamilyNamesLock)
            {
                names = RegisteredFontFamilyNamesCache;
                
                if (names != null)
                    return names;
                
                names = CreateFontFamilyNameSet(TypefaceProvider.GetTypefaces());
                RegisteredFontFamilyNamesCache = names;
                return names;
            }
        }
        
        private static HashSet<string> GetSystemFontFamilyNames()
        {
            var names = SystemFontFamilyNamesCache;
            
            if (names != null)
                return names;
            
            lock (FontFamilyNamesLock)
            {
                names = SystemFontFamilyNamesCache;
                
                if (names != null)
                    return names;
                
                names = CreateFontFamilyNameSet(SkFontManager.System.GetTypefaces());
                SystemFontFamilyNamesCache = names;
                return names;
            }
        }
        
        private static HashSet<string> CreateFontFamilyNameSet(IEnumerable<FontInfo> fonts)
        {
            return new HashSet<string>(fonts.Select(x => x.FamilyName), StringComparer.OrdinalIgnoreCase);
        }
        
        #endregion

        private static bool AreFontsFromDiscoveryPathRegistered { get; set; } = false;
        
        internal static void RegisterFontsFromDiscoveryPath()
        {
            const int maxFilesToScan = 100_000;
            
            if (AreFontsFromDiscoveryPathRegistered)
                return;
            
            RegisterFromDirectory(Settings.FontDiscoveryPath);
            
            #pragma warning disable CS0618 // directories added through the obsolete collection are still honored
            foreach (var fontDiscoveryPath in Settings.FontDiscoveryPaths)
                RegisterFromDirectory(fontDiscoveryPath);
            #pragma warning restore CS0618
            
            AreFontsFromDiscoveryPathRegistered = true;
            
            static void RegisterFromDirectory(string? path)
            {
                if (string.IsNullOrWhiteSpace(path))
                    return;
                
                var files = TryEnumerateFiles(path);

                if (files.Count == maxFilesToScan)
                {
                    throw new InvalidOperationException(
                        $"The library has reached the limit of {maxFilesToScan} files to scan for font files. " +
                        $"Please make sure that the {nameof(Settings)}.{nameof(Settings.FontDiscoveryPath)} setting " +
                        $"points to a directory that contains only the necessary files. " +
                        $"The reason of this exception is to prevent scanning too many files and avoid performance issues.");
                }
                
                foreach (var fontFile in files.FilterFontFiles())
                    TryRegisterFontFromFile(fontFile);
            }
            
            static ICollection<string> TryEnumerateFiles(string? path)
            {
                if (path == null)
                    return [];
            
                try
                {
                    if (!Directory.Exists(path))
                        return [];
                
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

        private static IEnumerable<string> FilterFontFiles(this IEnumerable<string> files)
        {
            var supportedFontExtensions = new[] { ".ttf", ".otf", ".ttc", ".pfb" };
            
            return files.Where(f => supportedFontExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));
        }
    }
}