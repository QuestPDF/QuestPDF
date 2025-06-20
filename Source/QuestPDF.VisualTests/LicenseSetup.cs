using QuestPDF.Infrastructure;

namespace QuestPDF.VisualTests
{
    [SetUpFixture]
    public class LicenseSetup
    {
        [OneTimeSetUp]
        public static void Setup()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }
    }
}