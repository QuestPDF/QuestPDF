using NUnit.Framework;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests
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