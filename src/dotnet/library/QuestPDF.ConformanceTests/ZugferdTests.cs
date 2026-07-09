using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class ZugferdTests
{
    [Test]
    public void ZugferdValidation_WithMustang()
    {
        var guid = Guid.NewGuid();
        var invoicePath = Path.Combine(Path.GetTempPath(), $"{guid}.pdf");

        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(60);

                    page.Content()
                        .Text("Conformance Test: ZUGFeRD")
                        .FontSize(24)
                        .FontColor(Colors.Blue.Darken2)
                        .Bold();
                });
            })
            .WithMetadata(new DocumentMetadata
            {
                Title = "Conformance Test: ZUGFeRD",
                Author = "SampleCompany",
                Subject = "ZUGFeRD Test Document",
                Language = "en-US"
            })
            .WithSettings(new DocumentSettings
            {
                PDFA_Conformance = PDFA_Conformance.PDFA_3B
            })
            .GeneratePdf(invoicePath);
        
        VeraPdfConformanceTestRunner.TestConformance(invoicePath);
        
        var zugferdInvoicePath = Path.Combine(Path.GetTempPath(), $"zugferd-{guid}.pdf");

        var facturPath = Path.Combine("Resources", "zugferd-factur-x.xml");
        var metadataPath = Path.Combine("Resources", "zugferd-xmp-metadata.xml");

        DocumentOperation
            .LoadFile(invoicePath)
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Key = "factur-zugferd",
                FilePath = facturPath,
                AttachmentName = "factur-x.xml",
                MimeType = "text/xml",
                Description = "Factur-X Invoice",
                Relationship = DocumentOperation.DocumentAttachmentRelationship.Source,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow
            })
            .ExtendMetadata(File.ReadAllText(metadataPath))
            .Save(zugferdInvoicePath);
        
        VeraPdfConformanceTestRunner.TestConformance(zugferdInvoicePath);
        MustangConformanceTestRunner.TestConformance(zugferdInvoicePath); 
    }
}
