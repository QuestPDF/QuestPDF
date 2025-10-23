using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

[TestFixture]
public class TableOfContentsTests
{
    [Test]
    [Ignore("For manual testing purposes only")]
    public void GenerateAndShow()
    {
        GetDocumentUnderTest().GeneratePdfAndShow();
    }
    
    [Test, TestCaseSource(typeof(TestHelpers), nameof(TestHelpers.PDFA_ConformanceLevels))]
    public void Test_PDFA(PDFA_Conformance conformance)
    {
        GetDocumentUnderTest()
            .WithSettings(new DocumentSettings
            {
                PDFA_Conformance = conformance
            })
            .TestConformance();
    }
    
    [Test, TestCaseSource(typeof(TestHelpers), nameof(TestHelpers.PDFUA_ConformanceLevels))]
    public void Test_PDFUA(PDFUA_Conformance conformance)
    {
        GetDocumentUnderTest()
            .WithSettings(new DocumentSettings
            {
                PDFUA_Conformance = conformance
            })
            .TestConformance();
    }

    private Document GetDocumentUnderTest()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(60);

                    page.Content()
                        .PaddingVertical(30)
                        .Column(column =>
                        {
                            column.Item()
                                .ExtendVertical()
                                .AlignMiddle()
                                .SemanticHeader1()
                                .Text("Conformance Test:\nTable of Contents")
                                .FontSize(36)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item().PageBreak();

                            column.Item().Element(GenerateTableOfContentsSection);

                            column.Item().PageBreak();

                            column.Item().Element(GeneratePlaceholderContentSection);
                        });
                });
            })
            .WithMetadata(new DocumentMetadata
            {
                Language = "en-US",
                Title = "Conformance Test", 
                Subject = "Table of Contents"
            });
    }

    private void GenerateTableOfContentsSection(IContainer container)
    {
        container
            .SemanticSection()
            .Column(column =>
            {
                column.Spacing(15);
                
                column
                    .Item()
                    .Text("Table of Contents")
                    .Bold()
                    .FontSize(20)
                    .FontColor(Colors.Blue.Medium);
                
                column.Item()
                    .SemanticTableOfContents()
                    .Column(column =>
                    {
                        column.Spacing(5);
                        
                        foreach (var i in Enumerable.Range(1, 10))
                        {
                            column.Item()
                                .SemanticTableOfContentsItem()
                                .SemanticLink($"Link to section {i}")
                                .SectionLink($"section-{i}")
                                .Row(row =>
                                {
                                    row.ConstantItem(25).Text($"{i}.");
                                    row.AutoItem().Text(Placeholders.Label());
                                    row.RelativeItem().PaddingHorizontal(2).TranslateY(11).LineHorizontal(1).LineDashPattern([1, 3]);
                                    row.AutoItem().Text(text => text.BeginPageNumberOfSection($"section-{i}"));
                                });
                        }
                    });
            });
    }
    
    private void GeneratePlaceholderContentSection(IContainer container)
    {
        container
            .Column(column =>
            {
                foreach (var i in Enumerable.Range(1, 10))
                {
                    column.Item()
                        .SemanticSection()
                        .Section($"section-{i}")
                        .Column(column =>
                        {
                            column.Spacing(15);
                            
                            column.Item()
                                .SemanticHeader2()
                                .Text($"Section {i}")
                                .Bold()
                                .FontSize(20)
                                .FontColor(Colors.Blue.Medium);
                            
                            column.Item().Text(Placeholders.Paragraph());
                            
                            foreach (var j in Enumerable.Range(1, i))
                            {
                                column.Item()
                                    .ArtifactOther()
                                    .Width(200)
                                    .Height(150)
                                    .CornerRadius(10)
                                    .Background(Placeholders.BackgroundColor());
                            }
                        });
                    
                    if (i < 10)
                        column.Item().PageBreak();
                }
            });
    }
}