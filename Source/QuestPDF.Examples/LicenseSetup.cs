using NUnit.Framework;
using QuestPDF.Infrastructure;

namespace QuestPDF.Examples
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