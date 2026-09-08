using QuestPDF.Infrastructure;

namespace QuestPDF.Benchmarks;

internal static class BenchmarkEnvironment
{
    public static void ConfigureQuestPdf()
    {
        Settings.License = LicenseType.Community;
        Settings.EnableDetailedLayoutErrors = false;
        Settings.UseSystemFonts = false;
        Settings.ThrowOnMissingTextGlyphs = false;
    }
}
