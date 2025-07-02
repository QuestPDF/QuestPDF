using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.VisualTests
{
    [SetUpFixture]
    public class TestsSetup
    {
        [OneTimeSetUp]
        public static void Setup()
        {
            var currentRuntime = NativeDependencyProvider.GetRuntimePlatform();

            if (currentRuntime != "osx-arm64")
            {
                Assert.Ignore(
                    "Visual tests are performed based on osx-arm64 runtime output. " +
                    "Each operating system (Windows, Linux, macOS) uses different font analysis and text rendering libraries. " +
                    "Moreover, each CPU architecture may produce slightly different floating-point calculation results. " +
                    "This may lead to different results even though the same code and configuration are used. " +
                    "Therefore, visual tests are not expected to pass entirely on other operating systems or CPU architectures.");
            }
                
            QuestPDF.Settings.License = LicenseType.Community;
            QuestPDF.Settings.UseEnvironmentFonts = false;
            
            VisualTestEngine.ClearActualOutputDirectories();
        }
    }
}