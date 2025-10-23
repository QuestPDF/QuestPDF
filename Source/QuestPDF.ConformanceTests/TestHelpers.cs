using ImageMagick;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

public class TestHelpers
{
    public static readonly IEnumerable<PDFA_Conformance> PDFA_ConformanceLevels = Enum.GetValues<PDFA_Conformance>().Skip(1);
    public static readonly IEnumerable<PDFUA_Conformance> PDFUA_ConformanceLevels = Enum.GetValues<PDFUA_Conformance>().Skip(1);

    static TestHelpers()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }
}