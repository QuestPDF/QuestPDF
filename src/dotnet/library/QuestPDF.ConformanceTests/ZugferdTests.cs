using QuestPDF.ConformanceTests.TestEngine;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ConformanceTests;

internal class ZugferdTests
{
    private static readonly string FacturPath = Path.Combine("Resources", "zugferd-factur-x.xml");
    private static readonly string MetadataPath = Path.Combine("Resources", "zugferd-xmp-metadata.xml");

    [TestCase(PDFA_Conformance.PDFA_3B, PDFUA_Conformance.None)]
    [TestCase(PDFA_Conformance.PDFA_3B, PDFUA_Conformance.PDFUA_1)]
    [TestCase(PDFA_Conformance.PDFA_3A, PDFUA_Conformance.PDFUA_1)]
    public void ZugferdValidation_WithMustang(PDFA_Conformance pdfaConformance, PDFUA_Conformance pdfuaConformance)
    {
        var guid = Guid.NewGuid();
        var invoicePath = Path.Combine(Path.GetTempPath(), $"{guid}.pdf");

        CreateInvoice(pdfaConformance, pdfuaConformance).GeneratePdf(invoicePath);
        VeraPdfConformanceTestRunner.TestConformance(invoicePath);

        var zugferdInvoicePath = Path.Combine(Path.GetTempPath(), $"zugferd-{guid}.pdf");

        DocumentOperation
            .LoadFile(invoicePath)
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Key = "factur-zugferd",
                FilePath = FacturPath,
                AttachmentName = "factur-x.xml",
                MimeType = "text/xml",
                Description = "Factur-X Invoice",
                Relationship = DocumentOperation.DocumentAttachmentRelationship.Source,
                CreationDate = DateTime.UtcNow,
                ModificationDate = DateTime.UtcNow
            })
            .ExtendMetadata(File.ReadAllText(MetadataPath))
            .Save(zugferdInvoicePath);

        VeraPdfConformanceTestRunner.TestConformance(zugferdInvoicePath);
        MustangConformanceTestRunner.TestConformance(zugferdInvoicePath);
    }

    /// <summary>
    /// In-memory attachments are embedded differently than file attachments, so the whole in-memory flow is validated as well.
    /// </summary>
    [Test]
    public void ZugferdValidation_InMemory_WithMustang()
    {
        var invoice = CreateInvoice(PDFA_Conformance.PDFA_3B, PDFUA_Conformance.PDFUA_1).GeneratePdf();

        var zugferdInvoice = DocumentOperation
            .LoadDocument(invoice)
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Key = "factur-zugferd",
                Content = File.ReadAllBytes(FacturPath),
                AttachmentName = "factur-x.xml",
                MimeType = "text/xml",
                Description = "Factur-X Invoice",
                Relationship = DocumentOperation.DocumentAttachmentRelationship.Source
            })
            .ExtendMetadata(File.ReadAllText(MetadataPath))
            .Save();

        // the validators accept only files
        var zugferdInvoicePath = Path.Combine(Path.GetTempPath(), $"zugferd-in-memory-{Guid.NewGuid()}.pdf");
        File.WriteAllBytes(zugferdInvoicePath, zugferdInvoice);

        VeraPdfConformanceTestRunner.TestConformance(zugferdInvoicePath);
        MustangConformanceTestRunner.TestConformance(zugferdInvoicePath);
    }

    private static IDocument CreateInvoice(PDFA_Conformance pdfaConformance, PDFUA_Conformance pdfuaConformance)
    {
        return Document
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
                PDFA_Conformance = pdfaConformance,
                PDFUA_Conformance = pdfuaConformance
            });
    }
}
