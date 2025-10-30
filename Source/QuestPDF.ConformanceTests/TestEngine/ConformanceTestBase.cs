using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests.TestEngine;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal abstract class ConformanceTestBase
{
    public static readonly IEnumerable<PDFA_Conformance> PDFA_ConformanceLevels = Enum.GetValues<PDFA_Conformance>().Skip(1);
    public static readonly IEnumerable<PDFUA_Conformance> PDFUA_ConformanceLevels = Enum.GetValues<PDFUA_Conformance>().Skip(1);
    
    [Test]
    [Explicit("Manual debugging only (override to enable)")]
    public void GenerateAndShow()
    {
        GetDocumentUnderTest()
            .WithMetadata(GetMetadata())
            .WithSettings(new DocumentSettings
            {
                PDFA_Conformance = PDFA_Conformance.PDFA_3A
            })
            .GeneratePdfAndShow();
    }
    
    [Test, TestCaseSource(nameof(PDFA_ConformanceLevels))]
    public void Test_PDFA(PDFA_Conformance conformance)
    {
        GetDocumentUnderTest()
            .WithMetadata(GetMetadata())
            .WithSettings(new DocumentSettings
            {
                PDFA_Conformance = conformance
            })
            .TestConformanceWithVeraPdf();
    }
    
    [Test, TestCaseSource(nameof(PDFUA_ConformanceLevels))]
    public void Test_PDFUA(PDFUA_Conformance conformance)
    {
        GetDocumentUnderTest()
            .WithMetadata(GetMetadata())
            .WithSettings(new DocumentSettings
            {
                PDFUA_Conformance = conformance
            })
            .TestConformanceWithVeraPdf();
    }
    
    [Test]
    public void TestSemanticMeaning()
    {
        var expectedSemanticTree = GetExpectedSemanticTree();
        GetDocumentUnderTest().TestSemanticTree(expectedSemanticTree);
    }

    private DocumentMetadata GetMetadata()
    {
        return new DocumentMetadata
        {
            Language = "en-US",
            Title = "Conformance Test",
            Subject = this.GetType().Name.Replace("Tests", string.Empty).PrettifyName()
        };
    }

    protected abstract Document GetDocumentUnderTest();
    
    protected abstract SemanticTreeNode? GetExpectedSemanticTree();
}