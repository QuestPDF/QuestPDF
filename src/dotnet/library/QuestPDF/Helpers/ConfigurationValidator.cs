using System.Diagnostics;

namespace QuestPDF.Helpers;

internal static class ConfigurationValidator
{
    private static bool IsConfigurationValidated = false;

    public static void ShowIfNeeded()
    {
        if (IsConfigurationValidated)
            return;

        WarnIfTextGlyphAvailabilityCheckIsEnabled();
        WarnIfFontFamilyAvailabilityCheckIsDisabled();
        WarnIfMultipleFontDiscoveryPathsAreConfigured();
        WarnIfDebuggingIsEnabled();
        WarnIfSystemFontsAreEnabled();

        IsConfigurationValidated = true;
    }
    
    private static void WarnIfTextGlyphAvailabilityCheckIsEnabled()
    {
        if (!Settings.ThrowOnMissingTextGlyphs)
            return;

        Trace.TraceWarning(
            "[QuestPDF] QuestPDF.Settings.ThrowOnMissingTextGlyphs is enabled. " +
            "QuestPDF will validate that the selected fonts contain every glyph used by your text. " +
            "When a glyph is missing, document generation stops with a detailed exception instead of rendering placeholder characters or empty areas. " +
            "This is especially helpful during development and for multilingual content, but in production it can surface unexpected font coverage issues as runtime exceptions. " +
            "Make sure the fonts deployed with your application cover all expected text, or disable this setting if you intentionally prefer generation to continue with missing or replacement glyphs. " +
            "By default, QuestPDF enables this setting only when a debugger is attached.");
    }

    private static void WarnIfFontFamilyAvailabilityCheckIsDisabled()
    {
        if (Settings.ThrowOnMissingFontFamilies)
            return;

        Trace.TraceWarning(
            "[QuestPDF] QuestPDF.Settings.ThrowOnMissingFontFamilies is disabled. " +
            "When a text style refers to a font family that is not registered with QuestPDF (and, if QuestPDF.Settings.UseSystemFonts is enabled, not installed on the system), QuestPDF silently renders the text with another available font instead of stopping document generation. " +
            "This hides configuration mistakes such as typos in font family names or font files missing from the deployment, and the substituted font can change the layout and appearance of documents. " +
            "For predictable output, keep this setting enabled and deploy the required fonts with your application. " +
            "By default, QuestPDF enables this setting.");
    }

    private static void WarnIfMultipleFontDiscoveryPathsAreConfigured()
    {
        if (Settings.FontDiscoveryPaths.Count <= 1)
            return;

        Trace.TraceWarning(
            "[QuestPDF] QuestPDF.Settings.FontDiscoveryPaths contains multiple paths. " +
            "Some QuestPDF font-loading operations can use only one font directory and will use the first path in this collection. " +
            "This does not stop document generation, but it can be confusing when fonts from later paths are not available in every rendering scenario. " +
            "For predictable results, use a single directory that contains all required fonts.");
    }

    private static void WarnIfDebuggingIsEnabled()
    {
        if (!Settings.EnableDebugging)
            return;

        Trace.TraceWarning(
            "[QuestPDF] QuestPDF.Settings.EnableDebugging is enabled. " +
            "QuestPDF will collect additional layout diagnostics and include more context when layout exceptions occur. " +
            "This is useful during development, but it can reduce performance and increase memory usage. " +
            "Disable this setting in production environments and during performance testing unless you specifically need diagnostic details. " +
            "By default, QuestPDF enables this setting only when a debugger is attached.");
    }

    private static void WarnIfSystemFontsAreEnabled()
    {
        if (!Settings.UseSystemFonts)
            return;

        Trace.TraceWarning(
            "[QuestPDF] QuestPDF.Settings.UseSystemFonts is enabled. " +
            "QuestPDF may use fonts installed on the current machine in addition to fonts registered with QuestPDF.Drawing.FontManager or discovered in QuestPDF.Settings.FontDiscoveryPaths. " +
            "This is convenient during development, but it can make documents depend on fonts that are not available in production, especially in minimal Docker images, serverless functions, and other reduced runtime environments. " +
            "As a result, the same document may fail to render or may use different fallback fonts after deployment. " +
            "For predictable output, deploy the required fonts with your application and keep QuestPDF.Settings.UseSystemFonts disabled. " +
            "By default, QuestPDF disables this setting.");
    }
}
