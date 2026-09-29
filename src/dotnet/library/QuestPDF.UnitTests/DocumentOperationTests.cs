using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

    private static void AssertPagesContainWatermark(string filePath, int expectedPageCount, int watermarkedPageCount)
    {
        AssertPagesContainWatermark(File.ReadAllBytes(filePath), expectedPageCount, watermarkedPageCount);
    }

    /// <summary>
    /// The sample documents do not use any XObjects on their own,
    /// while qpdf draws the overlay / underlay content as a form XObject on the target pages.
    /// The watermark pages are applied in sequence, so once they are exhausted,
    /// the remaining output pages stay unchanged.
    /// </summary>
    private static void AssertPagesContainWatermark(byte[] document, int expectedPageCount, int watermarkedPageCount)
    {
        using var inspector = PdfInspector.Load(document);

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
    public void NonAsciiCharactersAreSupported()
    {
        const string inputFileName = "Hallå där 🎉.pdf";
        const string outputFileName = "operation-non-ascii-🎯.pdf";
        const string userPassword = "zażółć gęślą jaźń";
        const string ownerPassword = "ελληνικά";
        const string attachmentFileName = "你好.pdf";
        const string attachmentKey = "Привет 🔑";

        GenerateSampleDocument(inputFileName, Colors.Red.Medium, 10);
        GenerateSampleDocument(attachmentFileName, Colors.Red.Medium, 10);

        DocumentOperation
            .LoadFile(inputFileName)
            .TakePages("2-5")
            .Encrypt(new DocumentOperation.Encryption128Bit()
            {
                UserPassword = userPassword,
                OwnerPassword = ownerPassword
            })
            .AddAttachment(new DocumentOperation.DocumentAttachment()
            {
                Key = attachmentKey,
                FilePath = attachmentFileName,
                AttachmentName = "こんにちは 안녕"
            })
            .Save(outputFileName);

        using var inspector = PdfInspector.Load(File.ReadAllBytes(outputFileName), userPassword);
        Assert.That(inspector.Pages.Count(), Is.EqualTo(4));
        
        var attachment = inspector.Root.GetProperty("attachments").GetProperty(attachmentKey);
        Assert.That(attachment.GetProperty("preferredname").GetString(), Is.EqualTo("こんにちは 안녕"));
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
    
    #region In-Memory Operations

    [Test]
    public void TakePagesInMemory()
    {
        var output = DocumentOperation
            .LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 10))
            .TakePages("2-5")
            .Save();

        using var inspector = PdfInspector.Load(output);
        Assert.That(inspector.Pages.Count(), Is.EqualTo(4));
    }

    [Test]
    public void MergeInMemory()
    {
        var output = DocumentOperation
            .LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 3))
            .MergeDocument(GenerateSampleDocument(Colors.Green.Medium, 5))
            .MergeDocument(GenerateSampleDocument(Colors.Blue.Medium, 7), "1-2")
            .Save();

        using var inspector = PdfInspector.Load(output);
        Assert.That(inspector.Pages.Count(), Is.EqualTo(3 + 5 + 2));
    }

    [Test]
    public void OverlayInMemory()
    {
        var output = DocumentOperation
            .LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 10))
            .OverlayFile(new DocumentOperation.LayerConfiguration
            {
                DocumentData = GenerateSampleDocument(Colors.Green.Medium, 5)
            })
            .Save();

        AssertPagesContainWatermark(output, expectedPageCount: 10, watermarkedPageCount: 5);
    }

    [Test]
    public void UnderlayInMemory()
    {
        var output = DocumentOperation
            .LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 10))
            .UnderlayFile(new DocumentOperation.LayerConfiguration
            {
                DocumentData = GenerateSampleDocument(Colors.Green.Medium, 5)
            })
            .Save();

        AssertPagesContainWatermark(output, expectedPageCount: 10, watermarkedPageCount: 5);
    }

    [Test]
    public void AttachmentFromContent()
    {
        var content = Encoding.UTF8.GetBytes("<invoice>Zażółć gęślą jaźń</invoice>");

        var output = DocumentOperation
            .LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 1))
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Content = content,
                AttachmentName = "invoice.xml"
            })
            .Save();

        using var inspector = PdfInspector.Load(output);

        // the key defaults to the attachment name, the same way it defaults to the file name for file attachments
        var attachment = inspector.Root.GetProperty("attachments").GetProperty("invoice.xml");
        Assert.That(attachment.GetProperty("preferredname").GetString(), Is.EqualTo("invoice.xml"));

        var stream = inspector.Resolve(attachment.GetProperty("preferredcontents"));
        Assert.That(inspector.GetStreamData(stream), Is.EqualTo(content));

        // the MIME type is derived from the extension of the attachment name
        Assert.That(stream.GetProperty("dict").GetProperty("/Subtype").GetString(), Is.EqualTo("/text/xml"));
    }

    [Test]
    public void AttachmentFromContentRequiresAttachmentName()
    {
        var operation = DocumentOperation.LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 1));

        var exception = Assert.Throws<ArgumentException>(() =>
        {
            operation.AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Key = "invoice",
                Content = [1, 2, 3]
            });
        });

        Assert.That(exception.Message, Does.Contain("AttachmentName"));
    }

    [Test]
    public void AttachmentRequiresSingleSource()
    {
        GenerateSampleDocument("attachment-single-source.pdf", Colors.Red.Medium, 1);
        var operation = DocumentOperation.LoadFile("attachment-single-source.pdf");

        Assert.Throws<ArgumentException>(() =>
        {
            operation.AddAttachment(new DocumentOperation.DocumentAttachment
            {
                FilePath = "attachment-single-source.pdf",
                Content = [1, 2, 3],
                AttachmentName = "data.bin"
            });
        });

        Assert.Throws<ArgumentException>(() =>
        {
            operation.AddAttachment(new DocumentOperation.DocumentAttachment
            {
                AttachmentName = "data.bin"
            });
        });
    }

    [Test]
    public void LayerRequiresSingleSource()
    {
        GenerateSampleDocument("layer-single-source.pdf", Colors.Red.Medium, 1);
        var operation = DocumentOperation.LoadFile("layer-single-source.pdf");

        Assert.Throws<ArgumentException>(() =>
        {
            operation.OverlayFile(new DocumentOperation.LayerConfiguration
            {
                FilePath = "layer-single-source.pdf",
                DocumentData = File.ReadAllBytes("layer-single-source.pdf")
            });
        });

        Assert.Throws<ArgumentException>(() =>
        {
            operation.UnderlayFile(new DocumentOperation.LayerConfiguration());
        });
    }

    [Test]
    public void DocumentDataIsValidated()
    {
        Assert.Throws<ArgumentNullException>(() => DocumentOperation.LoadDocument(null!));
        Assert.Throws<ArgumentException>(() => DocumentOperation.LoadDocument([]));

        var operation = DocumentOperation.LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 1));
        Assert.Throws<ArgumentNullException>(() => operation.MergeDocument(null!));
        Assert.Throws<ArgumentException>(() => operation.MergeDocument([]));
        Assert.Throws<ArgumentException>(() => operation.OverlayFile(new DocumentOperation.LayerConfiguration { DocumentData = [] }));
    }

    [Test]
    public void EncryptAndDecryptInMemory()
    {
        var encrypted = DocumentOperation
            .LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 3))
            .Encrypt(new DocumentOperation.Encryption256Bit
            {
                UserPassword = "user_password",
                OwnerPassword = "owner_password"
            })
            .Save();

        var decrypted = DocumentOperation
            .LoadDocument(encrypted, "owner_password")
            .Decrypt()
            .Save();

        using var inspector = PdfInspector.Load(decrypted);
        Assert.That(inspector.Root.GetProperty("encrypt").GetProperty("encrypted").GetBoolean(), Is.False);
        Assert.That(inspector.Pages.Count(), Is.EqualTo(3));
    }

    [Test]
    public void MixedFileAndInMemorySources()
    {
        GenerateSampleDocument("mixed-sources-main.pdf", Colors.Red.Medium, 4);
        GenerateSampleDocument("mixed-sources-merged.pdf", Colors.Blue.Medium, 2);

        DocumentOperation
            .LoadFile("mixed-sources-main.pdf")
            .MergeDocument(GenerateSampleDocument(Colors.Green.Medium, 3))
            .MergeFile("mixed-sources-merged.pdf")
            .OverlayFile(new DocumentOperation.LayerConfiguration
            {
                DocumentData = GenerateSampleDocument(Colors.Green.Medium, 2)
            })
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                FilePath = "mixed-sources-merged.pdf"
            })
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Content = Encoding.UTF8.GetBytes("Hello, World!"),
                AttachmentName = "message.txt"
            })
            .Save("operation-mixed-sources.pdf");

        AssertPagesContainWatermark("operation-mixed-sources.pdf", expectedPageCount: 4 + 3 + 2, watermarkedPageCount: 2);

        using var inspector = PdfInspector.Load(File.ReadAllBytes("operation-mixed-sources.pdf"));
        var attachments = inspector.Root.GetProperty("attachments");
        Assert.That(attachments.TryGetProperty("mixed-sources-merged.pdf", out _), Is.True);
        Assert.That(attachments.TryGetProperty("message.txt", out _), Is.True);
    }

    /// <summary>
    /// The stream receives the document in chunks of about 64 KiB, so a larger document checks that they are written in order.
    /// qpdf generates a new document ID for every output, and the ID has a constant length, so it is the only expected difference.
    /// </summary>
    [Test]
    public void SaveToStreamProducesSameDocumentAsSaveToFile()
    {
        var operation = DocumentOperation
            .LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 250))
            .MergeDocument(GenerateSampleDocument(Colors.Green.Medium, 250));

        operation.Save("operation-save-to-file.pdf");
        var fileOutput = File.ReadAllBytes("operation-save-to-file.pdf");

        using var stream = new MemoryStream();
        operation.Save(stream);
        var streamOutput = stream.ToArray();

        Assert.That(fileOutput.Length, Is.GreaterThan(4 * 64 * 1024));
        Assert.That(stream.CanWrite, Is.True, "The stream should not be closed");
        Assert.That(RemoveDocumentId(streamOutput), Is.EqualTo(RemoveDocumentId(fileOutput)));

        static string RemoveDocumentId(byte[] document)
        {
            var text = Encoding.Latin1.GetString(document);
            Assert.That(Regex.Count(text, @"/ID\s*\["), Is.EqualTo(1));
            return Regex.Replace(text, @"/ID\s*\[[^\]]*\]", "/ID []");
        }
    }

    [TestCase(0, TestName = "ExceptionThrownByOutputStreamIsPropagated(AtFirstWrite)")]
    [TestCase(100_000, TestName = "ExceptionThrownByOutputStreamIsPropagated(DuringWriting)")]
    public void ExceptionThrownByOutputStreamIsPropagated(int failAfterBytes)
    {
        // the document spans a few chunks, so the stream can accept the first one and fail on the next
        var operation = DocumentOperation.LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 250));

        var exception = Assert.Throws<IOException>(() => operation.Save(new FailingStream(failAfterBytes)));
        Assert.That(exception.Message, Is.EqualTo(FailingStream.ErrorMessage));
    }

    [Test]
    public void SaveToStreamRequiresWritableStream()
    {
        var operation = DocumentOperation.LoadDocument(GenerateSampleDocument(Colors.Red.Medium, 1));

        Assert.Throws<ArgumentNullException>(() => operation.Save((Stream)null!));
        Assert.Throws<ArgumentException>(() => operation.Save(new MemoryStream([], writable: false)));
    }

    /// <summary>
    /// qpdf error messages quote the names of in-memory inputs, so they describe which input is invalid.
    /// </summary>
    [Test]
    public void InvalidInMemoryDocumentIsNamedInErrorMessage()
    {
        var invalidDocument = Encoding.ASCII.GetBytes("This is not a PDF document.");

        var inputException = Assert.Catch(() => DocumentOperation.LoadDocument(invalidDocument).Save());
        Assert.That(inputException.Message, Does.Contain("qpdf-buffer://input"));

        var validDocument = GenerateSampleDocument(Colors.Red.Medium, 1);

        var mergeException = Assert.Catch(() =>
        {
            DocumentOperation
                .LoadDocument(validDocument)
                .MergeDocument(validDocument)
                .MergeDocument(invalidDocument)
                .Save();
        });

        Assert.That(mergeException.Message, Does.Contain("qpdf-buffer://merged-document-2"));
    }

    #endregion

    private static byte[] GenerateSampleDocument(Color color, int length)
    {
        return CreateSampleDocument(color, length).GeneratePdf();
    }

    private static void GenerateSampleDocument(string filePath, Color color, int length)
    {
        CreateSampleDocument(color, length).GeneratePdf(filePath);
    }

    private static IDocument CreateSampleDocument(Color color, int length)
    {
        return Document
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
                                .Text($"Page {i}")
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
            });
    }
} 