using System;

namespace QuestPDF
{
    public static class Settings
    {
        /// <summary>
        /// This value represents the maximum length of the document that the library produces.
        /// This is useful when layout constraints are too strong, e.g. one element does not fit in another.
        /// In such cases, the library would produce document of infinite length, consuming all available resources.
        /// To break the algorithm and save the environment, the library breaks the rendering process after reaching specified length of document.
        /// If your content requires generating longer documents, please assign the most reasonable value.
        /// </summary>
        public static int DocumentLayoutExceptionThreshold { get; set; } = 250;
        
        /// <summary>
        /// This flag generates additional document elements to cache layout calculation results.
        /// In the vast majority of cases, this significantly improves performance, while slightly increasing memory consumption.
        /// </summary>
        /// <remarks>By default, this flag is enabled only when the debugger is NOT attached.</remarks>
        public static bool EnableCaching { get; set; } = !System.Diagnostics.Debugger.IsAttached;
        
        /// <summary>
        /// This flag generates additional document elements to improve layout debugging experience.
        /// When the DocumentLayoutException is thrown, the library is able to provide additional execution context.
        /// It includes layout calculation results and path to the problematic area.
        /// </summary>
        /// <remarks>By default, this flag is enabled only when the debugger IS attached.</remarks>
        [Obsolete("The new implementation for debugging layout issues does not introduce any additional performance overhead. Therefore, this setting is no longer used since the 2023.1 release. Please remove this setter from your code.")]
        public static bool EnableDebugging { get; set; } = System.Diagnostics.Debugger.IsAttached;

        /// <summary>
        /// This flag enables checking the font glyph availability.
        /// If your text contains glyphs that are not present in the specified font,
        /// 1) when this flag is enabled: the DocumentDrawingException is thrown. OR 
        /// 2) when this flag is disabled: placeholder characters are visible in the produced PDF file. 
        /// Enabling this flag may slightly decrease document generation performance.
        /// However, it provides hints that used fonts are not sufficient to produce correct results.
        /// </summary>
        /// <remarks>By default, this flag is enabled only when the debugger IS attached.</remarks>
        public static bool CheckIfAllTextGlyphsAreAvailable { get; set; } = System.Diagnostics.Debugger.IsAttached;
    }
}