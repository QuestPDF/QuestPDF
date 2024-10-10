using System;
using System.Collections.Generic;
using QuestPDF.Helpers;
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
        /// <remarks>Enabled by default.</remarks>
        public static bool EnableCaching { get; set; } = true;
        
        /// <summary>
        /// This flag generates additional document elements to improve layout debugging experience.
        /// When the provided content contains size constraints impossible to meet, the library generates an enhanced exception message with additional location and layout measurement details.
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

        /// <summary>
        /// Decides whether the application should use the fonts available in the environment.
        /// </summary>
        /// <remarks>
        /// <para>When set to <c>true</c>, the application will use the fonts installed on the system where it is running. This is the default behavior.</para>
        /// <para>When set to <c>false</c>, the application will only use the fonts that have been registered using the <c>FontManager</c> class in the QuestPDF library.</para>
        /// <para>This property is useful when you want to control the fonts used by your application, especially in cases where the environment might not have the necessary fonts installed.</para>
        /// </remarks>
        public static bool UseEnvironmentFonts { get; set; } = true;
        
        /// <summary>
        /// Specifies the collection of paths where the library will automatically search for font files to register.
        /// </summary>
        /// <remarks>
        /// <para>By default, this collection contains the application files path.</para>
        /// <para>You can add additional paths to this collection to include more directories for automatic font registration.</para>
        /// </remarks>
        public static ICollection<string> FontDiscoveryPaths { get; } = new List<string>()
        {
            AppDomain.CurrentDomain.BaseDirectory
        };
        
        static Settings()
        {
            NativeDependencyCompatibilityChecker.Test();
        }
    }
}