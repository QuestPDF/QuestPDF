using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Skia;

namespace QuestPDF
{
    public static class Settings
    {
        /// <summary>
        /// <para>Selects the QuestPDF license tier that applies to your usage. Set this once at application startup, before generating the first document.</para>
        /// <para>For more details, please check the <a href="https://www.questpdf.com/pricing">QuestPDF Pricing webpage</a> and <a href="https://www.questpdf.com/license">QuestPDF License webpage</a>.</para>
        /// </summary>
        public static LicenseType? License { get; set; }
        
        /// <summary>
        /// This flag generates additional document elements to cache layout calculation results.
        /// In the vast majority of cases, this significantly improves performance, while slightly increasing memory consumption.
        /// </summary>
        /// <remarks>Enabled by default.</remarks>
        public static bool EnableCaching { get; set; } = true;
        
        /// <summary>
        /// This flag generates additional document elements to improve layout debugging experience.
        /// When the provided content contains size constraints impossible to meet, the library generates an enhanced exception message with additional location and layout measurement details.
        /// </summary>
        /// <remarks>By default, this flag is enabled only when the debugger IS attached.</remarks>  
        public static bool EnableDebugging { get; set; } = System.Diagnostics.Debugger.IsAttached;
        
        /// <summary>
        /// <para>Decides how the library reacts when the text contains glyphs that are not available in the used fonts, including the configured fallbacks.</para>
        /// <para>When this flag is enabled, document generation stops with the DocumentDrawingException.</para>
        /// <para>When this flag is disabled, document generation continues: missing glyphs are rendered as replacement characters or empty areas, and a warning listing them is written to the trace output.</para>
        /// </summary>
        /// <remarks>By default, this flag is enabled only when the debugger IS attached.</remarks>
        public static bool ThrowOnMissingTextGlyphs { get; set; } = System.Diagnostics.Debugger.IsAttached;
        
        /// <summary>
        /// <para>Decides how the library reacts when a text style refers to a font family that is not available: neither registered with the <c>FontManager</c> class (including fonts discovered in the <see cref="FontDiscoveryPaths"/> directories) nor, when <see cref="UseSystemFonts"/> is enabled, installed on the system.</para>
        /// <para>When this flag is enabled, document generation stops with the DocumentDrawingException.</para>
        /// <para>When this flag is disabled, document generation continues silently: the text is rendered with the first available font family from the fallback list, or with another registered font when none is available.</para>
        /// </summary>
        /// <remarks>By default, this flag is enabled only when the debugger IS attached.</remarks>
        public static bool ThrowOnMissingFontFamilies { get; set; } = System.Diagnostics.Debugger.IsAttached;

        /// <summary>
        /// Decides whether the library may use the fonts installed on the system where the application is running.
        /// </summary>
        /// <remarks>
        /// <para>When set to <c>false</c>, the library uses only the fonts registered with the <c>FontManager</c> class: fonts discovered automatically in the <see cref="FontDiscoveryPaths"/> directories and fonts registered manually. This makes the output independent of the runtime environment, especially where the necessary fonts might not be installed (e.g. minimal Docker images or serverless functions).</para>
        /// <para>When set to <c>true</c>, the library uses the system fonts in addition to the registered fonts. This is convenient during development, but the same document may render differently, or fail to render, after deployment to an environment with a different set of fonts installed.</para>
        /// <para>Referring to a font family that is not available is reported according to the <see cref="ThrowOnMissingFontFamilies"/> setting.</para>
        /// <para>Disabled by default. Before version 2026.8.1, this setting was enabled by default.</para>
        /// </remarks>
        public static bool UseSystemFonts { get; set; } = false;
        
        /// <summary>
        /// Specifies the collection of directories where the library automatically searches for font files to register (.ttf, .otf, .ttc and .pfb).
        /// </summary>
        /// <remarks>
        /// <para>By default, this collection contains the application directory. Font files deployed along with the application are therefore registered automatically, without calling the <c>FontManager</c> class.</para>
        /// <para>Add more paths to this collection to include additional directories. They are scanned recursively when the library is used for the first time, so configure this collection at application startup.</para>
        /// <para>Fonts discovered this way are always available to the library, regardless of the runtime environment (see <see cref="UseSystemFonts"/>).</para>
        /// </remarks>
        public static ICollection<string> FontDiscoveryPaths { get; } = new List<string>()
        {
            PathHelpers.ApplicationFilesPath
        };

        /// <summary>
        /// Gets or sets the file path used for temporary storage during the document generation process.
        /// This path is used by various operations that require temporary files.
        /// </summary>
        public static string? TemporaryStoragePath { get; set; }
        
        static Settings()
        {
            SkNativeDependencyCompatibilityChecker.Test();
        }
        
        #region Obsolete
        
        [Obsolete("This setting is ignored since the 2023.10 version. The new infinite layout detection algorithm works automatically. You can safely remove this setting from your codebase.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static int DocumentLayoutExceptionThreshold { get; set; } = 250;
        
        [Obsolete("This setting has been renamed since version 2026.9. Please use the ThrowOnMissingTextGlyphs property.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [ExcludeFromCodeCoverage]
        public static bool CheckIfAllTextGlyphsAreAvailable
        {
            get => ThrowOnMissingTextGlyphs;
            set => ThrowOnMissingTextGlyphs = value;
        }
        
        [Obsolete("This setting has been renamed since version 2026.9. Please use the UseSystemFonts property.")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [ExcludeFromCodeCoverage]
        public static bool UseEnvironmentFonts
        {
            get => UseSystemFonts;
            set => UseSystemFonts = value;
        }
        
        #endregion
    }
}