using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

[SetUpFixture]
public class LicenseSetup
{
    [OneTimeSetUp]
    public static void Setup()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }
}