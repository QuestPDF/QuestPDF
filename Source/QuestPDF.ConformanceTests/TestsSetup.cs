using System.Runtime.CompilerServices;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests
{
    public class TestsSetup
    {
        [ModuleInitializer]
        public static void Setup()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.UseEnvironmentFonts = false;
        }
    }
}