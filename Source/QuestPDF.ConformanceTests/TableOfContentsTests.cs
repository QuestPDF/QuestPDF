using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class TableOfContentsTests : ConformanceTestBase
{
    protected override Document GetDocumentUnderTest()
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
            });
        
        static void GenerateTableOfContentsSection(IContainer container)
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
        
        static void GeneratePlaceholderContentSection(IContainer container)
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
                                        .SemanticIgnore()
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
    
    protected override SemanticTreeNode? GetExpectedSemanticTree()
    {
        return ExpectedSemanticTree.DocumentRoot(root =>
        {
            root.Child("H1", h1 => h1.Alt("Conformance Test:\nTable of Contents"));

            // Table of Contents Section
            root.Child("Sect", sect =>
            {
                sect.Child("P");

                sect.Child("TOC", toc =>
                {
                    foreach (var i in Enumerable.Range(1, 10))
                    {
                        toc.Child("TOCI", toci =>
                        {
                            toci.Child("Link", link =>
                            {
                                link.Alt($"Link to section {i}");
                                link.Child("P"); // Number
                                link.Child("P"); // Label
                                link.Child("P"); // Page number
                            });
                        });
                    }
                });
            });

            // Content Sections
            foreach (var i in Enumerable.Range(1, 10))
            {
                root.Child("Sect", sect =>
                {
                    sect.Child("H2", h2 => h2.Alt($"Section {i}"));
                    sect.Child("P");
                });
            }
        });
    }
}