using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    /// <remarks>
    /// <para>The font family name and the weight and italic attributes are read from the font file. Use <see cref="GetRegisteredFonts"/> to learn the family names under which the registered fonts are available.</para>
    /// <para>Font family names are matched ignoring case.</para>
    /// </remarks>
    public static class FontManager
    {
        internal static SkTypefaceProvider TypefaceProvider { get; } = new();
        internal static SkFontManager? SystemFontManager => Settings.UseSystemFonts ? SkFontManager.System : null;

        static FontManager()
        {
            SkNativeDependencyCompatibilityChecker.Test();
            RegisterFontsFromDiscoveryPath();
        }
        
        #region Font Registration
        
        /// <summary>
        /// Registers a font (.ttf, .otf, .ttc or .pfb) from a stream. The font family name and all related attributes are detected automatically.
        /// <a href="https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration">Learn more</a>
        /// </summary>
        /// <param name="stream">Stream with the font file content.</param>
        public static void RegisterFontFromStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            
            RegisterTypeface(memoryStream.ToArray());
        }
        
        /// <summary>
        /// Registers a font (.ttf, .otf, .ttc or .pfb) from its binary content. The font family name and all related attributes are detected automatically.
        /// <a href="https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration">Learn more</a>
        /// </summary>
        /// <param name="data">Content of the font file.</param>
        public static void RegisterFontFromBinaryData(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            
            RegisterTypeface(data);
        }
        
        /// <summary>
        /// Registers a font (.ttf, .otf, .ttc or .pfb) from a file. The font family name and all related attributes are detected automatically.
        /// <a href="https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration">Learn more</a>
        /// </summary>
        /// <param name="path">Path to the font file. Absolute paths are used as-is. Relative paths are resolved first against the current working directory, and then against the application directory.</param>
        public static void RegisterFontFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            
            path = PathHelpers.ResolveResourceFilePath(path);
            RegisterTypeface(File.ReadAllBytes(path));
        }
        
        /// <summary>
        /// Registers a font (.ttf, .otf, .ttc or .pfb) from a resource embedded in the calling assembly. The font family name and all related attributes are detected automatically.
        /// <a href="https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration">Learn more</a>
        /// </summary>
        /// <remarks>
        /// The resource is searched in the assembly that calls this method. When the font is embedded in a different assembly, e.g. a shared library, use the overload that accepts the <see cref="Assembly"/> instance.
        /// </remarks>
        /// <param name="resourceName">Case-sensitive name of the manifest resource, e.g. "MyApp.Fonts.MyFont.ttf".</param>
        [MethodImpl(MethodImplOptions.NoInlining)] // the calling assembly must be the one that invoked this method
        public static void RegisterFontFromEmbeddedResource(string resourceName)
        {
            RegisterFontFromEmbeddedResource(Assembly.GetCallingAssembly(), resourceName);
        }
        
        /// <summary>
        /// Registers a font (.ttf, .otf, .ttc or .pfb) from a resource embedded in the provided assembly. The font family name and all related attributes are detected automatically.
        /// <a href="https://www.questpdf.com/api-reference/text/font-management.html#manual-font-registration">Learn more</a>
        /// </summary>
        /// <param name="assembly">Assembly containing the embedded resource, e.g. <c>typeof(Program).Assembly</c>.</param>
        /// <param name="resourceName">Case-sensitive name of the manifest resource, e.g. "MyApp.Fonts.MyFont.ttf".</param>
        public static void RegisterFontFromEmbeddedResource(Assembly assembly, string resourceName)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));
            
            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentNullException(nameof(resourceName));
            
            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
                throw new ArgumentException($"Cannot load font file from an embedded resource. Please make sure that the resource is available in the '{assembly.GetName().Name}' assembly or the path is correct: {resourceName}");
            
            RegisterFontFromStream(stream);
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
                RegisterFontFromFile(fontFile);
        }
        
        private static void RegisterTypeface(byte[] data, string? customFamilyName = null)
        {
            lock (FontFamilyNamesLock)
            {
                using var fontData = SkData.FromBinary(data);
                
                TypefaceProvider.AddTypefaceFromData(fontData, customFamilyName);
                RegisteredFontFamilyNamesCache = null;
            }
        }

        #endregion
        
        #region Font Enumeration
        
        /// <summary>
        /// Returns information about the fonts registered in the library: fonts discovered automatically in the <see cref="Settings.FontDiscoveryPath"/> directory,
        /// and fonts registered manually with the <c>RegisterFont*</c> methods.
        /// These fonts are always available to the library, regardless of the runtime environment.
        /// </summary>
        /// <remarks>
        /// <para>Each entry describes a single typeface (font face), e.g. the regular and bold faces of one family are listed separately.</para>
        /// <para>A typeface available under several family names (e.g. localized names) is listed once per name.</para>
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
        
        #endregion
        
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
        
        #region Font Discovery

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
                    RegisterFontFromFile(fontFile);
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
        
        #endregion
        
        #region Obsolete
        
        [Obsolete("Since version 2026.9 registering fonts under a custom name is no longer supported. Please use the RegisterFontFromStream method and refer to the font by the family name stored in the font file (see the GetRegisteredFonts method).")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [ExcludeFromCodeCoverage]
        public static void RegisterFontType(string fontName, Stream stream)
        {
            RegisterFontWithCustomName(fontName, stream);
        }
        
        [Obsolete("Since version 2026.9 registering fonts under a custom name is no longer supported. Please use the RegisterFontFromStream method and refer to the font by the family name stored in the font file (see the GetRegisteredFonts method).")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [ExcludeFromCodeCoverage]
        public static void RegisterFontWithCustomName(string fontName, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            
            RegisterTypeface(memoryStream.ToArray(), fontName);
        }
        
        [Obsolete("Since version 2026.9 this method has been renamed. Please use the RegisterFontFromStream method.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [ExcludeFromCodeCoverage]
        public static void RegisterFont(Stream stream)
        {
            RegisterFontFromStream(stream);
        }
        
        #endregion
    }
}
