using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

public class AccessibilityExamples
{
    [Test]
    public void MinimalExample()
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A5);
                    page.Margin(30);

                    page.Header()
                        .PaddingBottom(15)
                        .SemanticHeader1()
                        .Text("Accessibility Test Document")
                        .FontColor(Colors.Blue.Darken3)
                        .FontSize(24)
                        .Bold();
                    
                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(20);
                            
                            column.Item()
                                .SemanticSection()
                                .Column(column =>
                                {
                                    column.Item()
                                        .PaddingBottom(10)
                                        .SemanticHeader2()
                                        .Text("Section with text content")
                                        .FontColor(Colors.Blue.Darken1)
                                        .FontSize(16);
                                    
                                    column.Item()
                                        .Text(Placeholders.Paragraphs())
                                        .FontSize(12)
                                        .ParagraphSpacing(8);
                                });
                            
                            column.Item()
                                .PreventPageBreak()
                                .SemanticSection()
                                .Column(column =>
                                {
                                    column.Item()
                                        .PaddingBottom(10)
                                        .SemanticHeader2()
                                        .Text("Section with image")
                                        .FontColor(Colors.Blue.Darken1)
                                        .FontSize(16);
                                    
                                    column.Item()
                                        .Width(250)
                                        .SemanticImage("Image showing a laptop")
                                        .Image("Resources/product.jpg");
                                });
                        });
                });
            })
            .WithMetadata(new DocumentMetadata
            {
                Language = "en-US",
                Title = "Accessibility Test",
                Subject = "This document shows how easy it is to create accessible PDF documents with QuestPDF"
            })
            .WithSettings(new DocumentSettings
            {
                PDFA_Conformance = PDFA_Conformance.PDFA_3A,
                PDFUA_Conformance = PDFUA_Conformance.PDFUA_1
            })
            .GeneratePdf("accessibility-minimal-example.pdf");
    }
}