using System;
using QuestPDF.Infrastructure;

namespace QuestPDF
{
    public static class Settings
    {
        /// <summary>
        /// <para>Please kindly select license type that applies to your usage of the QuestPDF library.</para>
        /// <para>For more details, please check the <a href="https://www.questpdf.com/pricing.html">QuestPDF License and Pricing webpage</a></para>
        /// </summary>
        public static LicenseType? License { get; set; }
        
        [Obsolete("This setting is ignored since the 2023.10 version. The new infinite layout detection algorithm works automatically. You can safely remove this setting from your codebase.")]
        public static int DocumentLayoutExceptionThreshold { get; set; } = 250;
        
        /// <summary>
        /// This flag generates additional document elements to cache layout calculation results.
        /// In the vast majority of cases, this significantly improves performance, while slightly increasing memory consumption.
        /// </summary>
        /// <remarks>By default, this flag is enabled only when the debugger is NOT attached.</remarks>
        public static bool EnableCaching { get; set; } = !System.Diagnostics.Debugger.IsAttached;
        
        /// <summary>
        /// This flag generates additional document elements to improve layout debugging experience.
        /// When the provided content contains size constraints impossible to meet, the library generates special visual annotations to help determining the root cause.
        /// </summary>
        /// <remarks>By default, this flag is enabled only when the debugger IS attached.</remarks>  
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