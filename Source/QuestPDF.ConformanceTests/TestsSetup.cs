using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public static void Setup()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.UseEnvironmentFonts = false;
        }
    }
}