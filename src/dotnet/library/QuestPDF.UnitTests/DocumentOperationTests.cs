using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

/// <summary>
/// This test suite focuses on executing various QPDF operations.
/// Each test checks the primary effect of the operation using the qpdf JSON representation of the output document.
/// </summary>
public class DocumentOperationTests
{
    [Test]
    public void TakePages()
    {
        GenerateSampleDocument("take-input.pdf", Colors.Red.Medium, 10);
        
        DocumentOperation
            .LoadFile("take-input.pdf")
            .TakePages("2-5")
            .Save("operation-take.pdf");

        using var inspector = PdfInspector.Load(File.ReadAllBytes("operation-take.pdf"));
        Assert.That(inspector.Pages.Count(), Is.EqualTo(4));
    }
    
    [Test]
    public void MergeTest()
    {
        GenerateSampleDocument("merge-first.pdf", Colors.Red.Medium, 3);
        GenerateSampleDocument("merge-second.pdf", Colors.Green.Medium, 5);
        GenerateSampleDocument("merge-third.pdf", Colors.Blue.Medium, 7);
        
        DocumentOperation
            .LoadFile("merge-first.pdf")
            .MergeFile("merge-second.pdf")
            .MergeFile("merge-third.pdf")
            .Save("operation-merged.pdf");

        using var inspector = PdfInspector.Load(File.ReadAllBytes("operation-merged.pdf"));
        Assert.That(inspector.Pages.Count(), Is.EqualTo(3 + 5 + 7));
    }
    
    [Test]
    public void OverlayTest()
    {
        GenerateSampleDocument("overlay-main.pdf", Colors.Red.Medium, 10);
        GenerateSampleDocument("overlay-watermark.pdf", Colors.Green.Medium, 5);
        
        DocumentOperation
            .LoadFile("overlay-main.pdf")
            .OverlayFile(new DocumentOperation.LayerConfiguration
            {
                FilePath = "overlay-watermark.pdf"
            })
            .Save("operation-overlay.pdf");

        AssertPagesContainWatermark("operation-overlay.pdf", expectedPageCount: 10, watermarkedPageCount: 5);
    }
    
    [Test]
    public void UnderlayTest()
    {
        GenerateSampleDocument("underlay-main.pdf", Colors.Red.Medium, 10);
        GenerateSampleDocument("underlay-watermark.pdf", Colors.Green.Medium, 5);
        
        DocumentOperation
            .LoadFile("underlay-main.pdf")
            .UnderlayFile(new DocumentOperation.LayerConfiguration
            {
                FilePath = "underlay-watermark.pdf",
            })
            .Save("operation-underlay.pdf");

        AssertPagesContainWatermark("operation-underlay.pdf", expectedPageCount: 10, watermarkedPageCount: 5);
    }

    /// <summary>
    /// The sample documents do not use any XObjects on their own,
    /// while qpdf draws the overlay / underlay content as a form XObject on the target pages.
    /// The watermark pages are applied in sequence, so once they are exhausted,
    /// the remaining output pages stay unchanged.
    /// </summary>
    private static void AssertPagesContainWatermark(string filePath, int expectedPageCount, int watermarkedPageCount)
    {
        using var inspector = PdfInspector.Load(File.ReadAllBytes(filePath));

        var pages = inspector.Pages.ToList();
        Assert.That(pages, Has.Count.EqualTo(expectedPageCount));

        foreach (var (page, pageIndex) in pages.Select((page, pageIndex) => (page, pageIndex)))
        {
            var resources = inspector.Resolve(page.GetProperty("/Resources"));
            Assert.That(resources.TryGetProperty("/XObject", out _), Is.EqualTo(pageIndex < watermarkedPageCount));
        }
    }

    [Test]
    public void AttachmentTest()
    {
        GenerateSampleDocument("attachment-main.pdf", Colors.Red.Medium, 10);
        GenerateSampleDocument("attachment-file.pdf", Colors.Green.Medium, 5);
        
        DocumentOperation
            .LoadFile("attachment-main.pdf")
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                FilePath = "attachment-file.pdf"
            })
            .Save("operation-attachment.pdf");

        using var inspector = PdfInspector.Load(File.ReadAllBytes("operation-attachment.pdf"));
        Assert.That(inspector.Root.GetProperty("attachments").TryGetProperty("attachment-file.pdf", out _), Is.True);
    }
    
    [Test]
    public void Encrypt40Test()
    {
        GenerateSampleDocument("encrypt40-input.pdf", Colors.Red.Medium, 10);
        
        DocumentOperation
            .LoadFile("encrypt40-input.pdf")
            .Encrypt(new DocumentOperation.Encryption40Bit()
            {
                UserPassword = "user_password",
                OwnerPassword = "owner_password"
            })
            .Save("operation-encrypt40.pdf");

        AssertEncryptionRevision("operation-encrypt40.pdf", expectedRevision: 2);
    }
    
    [Test]
    public void Encrypt128Test()
    {
        GenerateSampleDocument("encrypt128-input.pdf", Colors.Red.Medium, 10);
        
        DocumentOperation
            .LoadFile("encrypt128-input.pdf")
            .Encrypt(new DocumentOperation.Encryption128Bit()
            {
                UserPassword = "user_password",
                OwnerPassword = "owner_password"
            })
            .Save("operation-encrypt128.pdf");

        AssertEncryptionRevision("operation-encrypt128.pdf", expectedRevision: 4);
    }
    
    [Test]
    public void Encrypt256Test()
    {
        GenerateSampleDocument("encrypt256-input.pdf", Colors.Red.Medium, 10);
        
        DocumentOperation
            .LoadFile("encrypt256-input.pdf")
            .Encrypt(new DocumentOperation.Encryption256Bit()
            {
                UserPassword = "user_password",
                OwnerPassword = "owner_password"
            })
            .Save("operation-encrypt256.pdf");

        AssertEncryptionRevision("operation-encrypt256.pdf", expectedRevision: 6);
    }

    /// <summary>
    /// Checks that the document is encrypted using the expected standard security handler revision:
    /// 2 for 40-bit RC4, 4 for 128-bit AES, 6 for 256-bit AES.
    /// </summary>
    private static void AssertEncryptionRevision(string filePath, int expectedRevision)
    {
        using var inspector = PdfInspector.Load(File.ReadAllBytes(filePath), password: "user_password");

        var encrypt = inspector.Root.GetProperty("encrypt");
        Assert.That(encrypt.GetProperty("encrypted").GetBoolean(), Is.True);
        Assert.That(encrypt.GetProperty("parameters").GetProperty("R").GetInt32(), Is.EqualTo(expectedRevision));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Encrypt40AllowModificationTest(bool allowModification)
    {
        GenerateSampleDocument("encrypt40-modify-input.pdf", Colors.Red.Medium, 1);

        var outputPath = $"operation-encrypt40-modify-{allowModification}.pdf";

        DocumentOperation
            .LoadFile("encrypt40-modify-input.pdf")
            .Encrypt(new DocumentOperation.Encryption40Bit()
            {
                UserPassword = "",
                OwnerPassword = "owner_password",
                AllowModification = allowModification
            })
            .Save(outputPath);

        Assert.That(ReadEncryptionCapability(outputPath, "modifyother"), Is.EqualTo(allowModification));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Encrypt40AllowPrintingTest(bool allowPrinting)
    {
        GenerateSampleDocument("encrypt40-print-input.pdf", Colors.Red.Medium, 1);

        var outputPath = $"operation-encrypt40-print-{allowPrinting}.pdf";

        DocumentOperation
            .LoadFile("encrypt40-print-input.pdf")
            .Encrypt(new DocumentOperation.Encryption40Bit()
            {
                UserPassword = "",
                OwnerPassword = "owner_password",
                AllowPrinting = allowPrinting
            })
            .Save(outputPath);

        Assert.That(ReadEncryptionCapability(outputPath, "printhigh"), Is.EqualTo(allowPrinting));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Encrypt128AllowModificationTest(bool allowModification)
    {
        GenerateSampleDocument("encrypt128-modify-input.pdf", Colors.Red.Medium, 1);

        var outputPath = $"operation-encrypt128-modify-{allowModification}.pdf";

        DocumentOperation
            .LoadFile("encrypt128-modify-input.pdf")
            .Encrypt(new DocumentOperation.Encryption128Bit()
            {
                UserPassword = "",
                OwnerPassword = "owner_password",
                AllowModification = allowModification
            })
            .Save(outputPath);

        Assert.That(ReadEncryptionCapability(outputPath, "modifyother"), Is.EqualTo(allowModification));
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Encrypt256AllowModificationTest(bool allowModification)
    {
        GenerateSampleDocument("encrypt256-modify-input.pdf", Colors.Red.Medium, 1);

        var outputPath = $"operation-encrypt256-modify-{allowModification}.pdf";

        DocumentOperation
            .LoadFile("encrypt256-modify-input.pdf")
            .Encrypt(new DocumentOperation.Encryption256Bit()
            {
                UserPassword = "",
                OwnerPassword = "owner_password",
                AllowModification = allowModification
            })
            .Save(outputPath);

        Assert.That(ReadEncryptionCapability(outputPath, "modifyother"), Is.EqualTo(allowModification));
    }

    [TestCase(40)]
    [TestCase(128)]
    [TestCase(256)]
    public void EncryptWithoutUserPasswordTest(int encryptionLevel)
    {
        GenerateSampleDocument("encrypt-no-user-password-input.pdf", Colors.Red.Medium, 1);

        var outputPath = $"operation-encrypt{encryptionLevel}-no-user-password.pdf";

        var operation = DocumentOperation.LoadFile("encrypt-no-user-password-input.pdf");

        operation = encryptionLevel switch
        {
            40 => operation.Encrypt(new DocumentOperation.Encryption40Bit { OwnerPassword = "owner_password" }),
            128 => operation.Encrypt(new DocumentOperation.Encryption128Bit { OwnerPassword = "owner_password" }),
            256 => operation.Encrypt(new DocumentOperation.Encryption256Bit { OwnerPassword = "owner_password" }),
            _ => throw new ArgumentOutOfRangeException(nameof(encryptionLevel))
        };

        operation.Save(outputPath);

        // the output document should be encrypted, yet possible to open without providing any password
        using var inspector = PdfInspector.Load(File.ReadAllBytes(outputPath));
        Assert.That(inspector.Root.GetProperty("encrypt").GetProperty("encrypted").GetBoolean(), Is.True);
    }

    /// <summary>
    /// Reads the effective encryption permission as reported by qpdf,
    /// e.g. "modifyother" for the "modify contents" permission, or "printhigh" for printing.
    /// </summary>
    private static bool ReadEncryptionCapability(string filePath, string capability)
    {
        using var inspector = PdfInspector.Load(File.ReadAllBytes(filePath));

        return inspector.Root
            .GetProperty("encrypt")
            .GetProperty("capabilities")
            .GetProperty(capability)
            .GetBoolean();
    }

    [Test]
    public void LinearizeTest()
    {
        GenerateSampleDocument("linearize-input.pdf", Colors.Red.Medium, 10);
        
        DocumentOperation
            .LoadFile("linearize-input.pdf")
            .Linearize()
            .Save("operation-linearize.pdf");

        // the linearization dictionary is always located at the beginning of the file
        var fileHeader = System.Text.Encoding.Latin1.GetString(File.ReadAllBytes("operation-linearize.pdf"), 0, 1024);
        Assert.That(fileHeader, Does.Contain("/Linearized"));
    }
    
    [Test]
    public void DecryptTest()
    {
        GenerateSampleDocument("decrypt-input-not-encrypted.pdf", Colors.Red.Medium, 10);
        
        DocumentOperation
            .LoadFile("decrypt-input-not-encrypted.pdf")
            .Encrypt(new DocumentOperation.Encryption256Bit()
            {
                UserPassword = "user_password",
                OwnerPassword = "owner_password"
            })
            .Save("decrypt-input-encrypted.pdf");
        
        DocumentOperation
            .LoadFile("decrypt-input-encrypted.pdf", "owner_password")
            .Decrypt()
            .Save("operation-decrypt.pdf");

        using var inspector = PdfInspector.Load(File.ReadAllBytes("operation-decrypt.pdf"));
        Assert.That(inspector.Root.GetProperty("encrypt").GetProperty("encrypted").GetBoolean(), Is.False);
    }
    
    [Test]
    public void RemoveRestrictionsTest()
    {
        GenerateSampleDocument("remove-restrictions-input-not-encrypted.pdf", Colors.Red.Medium, 10);
        
        DocumentOperation
            .LoadFile("remove-restrictions-input-not-encrypted.pdf")
            .Encrypt(new DocumentOperation.Encryption256Bit()
            {
                UserPassword = string.Empty,
                OwnerPassword = "owner_password",
                AllowPrinting = false,
                AllowContentExtraction = false
            })
            .Save("remove-restrictions-input-encrypted.pdf");
        
        DocumentOperation
            .LoadFile("remove-restrictions-input-encrypted.pdf", "owner_password")
            .RemoveRestrictions()
            .Save("operation-remove-restrictions.pdf");

        using var inspector = PdfInspector.Load(File.ReadAllBytes("operation-remove-restrictions.pdf"));
        Assert.That(inspector.Root.GetProperty("encrypt").GetProperty("encrypted").GetBoolean(), Is.False);
    }
    
    [Test]
    public void LoadEncryptedWithIncorrectPasswordTest()
    {
        GenerateSampleDocument("load-encrypted-input-not-encrypted.pdf", Colors.Red.Medium, 10);
        
        DocumentOperation
            .LoadFile("load-encrypted-input-not-encrypted.pdf")
            .Encrypt(new DocumentOperation.Encryption256Bit()
            {
                UserPassword = "user_password",
                OwnerPassword = "owner_password"
            })
            .Save("load-encrypted-input-encrypted.pdf");
        
        Assert.Catch(() =>
        {
            DocumentOperation
                .LoadFile("load-encrypted-input-encrypted.pdf", "wrong_password")
                .Save("operation-load-encrypted.pdf");
        });
    }
    
    [Test]
    public void ExtendMetadataTest()
    {
        GenerateSampleDocument("extend-metadata-input.pdf", Colors.Red.Medium, 10);
        
        // requires PDF/A-3b
        DocumentOperation
            .LoadFile("extend-metadata-input.pdf")
            .ExtendMetadata("<rdf:Description xmlns:dc=\"http://purl.org/dc/elements/1.1/\" rdf:about=\"\"></rdf:Description>")
            .Save("operation-extend-metadata.pdf");

        using var inspector = PdfInspector.Load(File.ReadAllBytes("operation-extend-metadata.pdf"));

        var catalog = inspector.Resolve(inspector.Trailer.GetProperty("/Root"));
        var metadata = inspector.GetStreamText(catalog.GetProperty("/Metadata"));
        Assert.That(metadata, Does.Contain("http://purl.org/dc/elements/1.1/"));
    }
    
    private void GenerateSampleDocument(string filePath, Color color, int length)
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.Transparent);
                    
                    page.Content().Column(column =>
                    {
                        foreach (var i in Enumerable.Range(1, length))
                        {
                            if (i != 1)
                                column.Item().PageBreak();
                            
                            var width = Random.Shared.Next(100, 200);
                            var height = Random.Shared.Next(100, 200);
                            
                            var horizontalTranslation = Random.Shared.Next(0, (int)PageSizes.A4.Width - width);
                            var verticalTranslation = Random.Shared.Next(0, (int)PageSizes.A4.Height - height);
                            
                            column.Item()
                                .OffsetX(horizontalTranslation)
                                .OffsetY(verticalTranslation)
                                .Width(width)
                                .Height(height)
                                .Background(color.WithAlpha(64))
                                .AlignCenter()
                                .AlignMiddle()
                                .Text($"{filePath}\npage {i}")
                                .FontColor(color)
                                .Bold()
                                .FontSize(16);
                        }
                    });
                });
            })
            .WithSettings(new DocumentSettings
            {
                PDFA_Conformance = PDFA_Conformance.PDFA_3B
            })
            .GeneratePdf(filePath);
    }
} 