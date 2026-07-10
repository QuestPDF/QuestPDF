using QuestPDF.Infrastructure;

namespace QuestPDF.Benchmarks;

internal static class BenchmarkEnvironment
{
    public static void ConfigureQuestPdf()
    {
        Settings.License = LicenseType.Community;
        Settings.EnableDebugging = false;
        Settings.UseEnvironmentFonts = false;
        Settings.CheckIfAllTextGlyphsAreAvailable = false;
    }
}
