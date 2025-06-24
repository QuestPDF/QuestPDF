using QuestPDF.Infrastructure;

namespace QuestPDF.VisualTests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public static void Setup()
        {
            VisualTestEngine.ClearActualOutputDirectories();
            QuestPDF.Settings.License = LicenseType.Community;
        }
    }
}