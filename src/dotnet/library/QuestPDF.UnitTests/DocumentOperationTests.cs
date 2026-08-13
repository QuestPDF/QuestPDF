using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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

        AssertPagesContainWatermark(File.ReadAllBytes("operation-overlay.pdf"), expectedPageCount: 10, watermarkedPageCount: 5);
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

        AssertPagesContainWatermark(File.ReadAllBytes("operation-underlay.pdf"), expectedPageCount: 10, watermarkedPageCount: 5);
    }

    /// <summary>
    /// The sample documents do not use any XObjects on their own,
    /// while qpdf draws the overlay / underlay content as a form XObject on the target pages.
    /// The watermark pages are applied in sequence, so once they are exhausted,
    /// the remaining output pages stay unchanged.
    /// </summary>
    private static void AssertPagesContainWatermark(byte[] documentData, int expectedPageCount, int watermarkedPageCount)
    {
        using var inspector = PdfInspector.Load(documentData);

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

    /// <summary>
    /// The in-memory API performs document operations without creating any temporary files:
    /// inputs are passed to qpdf as memory buffers, and the output is streamed back through a callback.
    /// </summary>
    [Test]
    public void TakePagesInMemory()
    {
        var input = GenerateSampleDocumentData("take-memory-input", Colors.Red.Medium, 10);

        var result = DocumentOperation
            .LoadDocument(input)
            .TakePages("2-5")
            .Save();

        using var inspector = PdfInspector.Load(result);
        Assert.That(inspector.Pages.Count(), Is.EqualTo(4));
    }

    [Test]
    public void MergeInMemoryTest()
    {
        var first = GenerateSampleDocumentData("merge-memory-first", Colors.Red.Medium, 3);
        var second = GenerateSampleDocumentData("merge-memory-second", Colors.Green.Medium, 5);
        var third = GenerateSampleDocumentData("merge-memory-third", Colors.Blue.Medium, 7);

        var result = DocumentOperation
            .LoadDocument(first)
            .MergeDocument(second)
            .MergeDocument(third)
            .Save();

        using var inspector = PdfInspector.Load(result);
        Assert.That(inspector.Pages.Count(), Is.EqualTo(3 + 5 + 7));
    }

    [Test]
    public void OverlayInMemoryTest()
    {
        var main = GenerateSampleDocumentData("overlay-memory-main", Colors.Red.Medium, 10);
        var watermark = GenerateSampleDocumentData("overlay-memory-watermark", Colors.Green.Medium, 5);

        var result = DocumentOperation
            .LoadDocument(main)
            .OverlayFile(new DocumentOperation.LayerConfiguration
            {
                DocumentData = watermark
            })
            .Save();

        AssertPagesContainWatermark(result, expectedPageCount: 10, watermarkedPageCount: 5);
    }

    [Test]
    public void UnderlayInMemoryTest()
    {
        var main = GenerateSampleDocumentData("underlay-memory-main", Colors.Red.Medium, 10);
        var watermark = GenerateSampleDocumentData("underlay-memory-watermark", Colors.Green.Medium, 5);

        var result = DocumentOperation
            .LoadDocument(main)
            .UnderlayFile(new DocumentOperation.LayerConfiguration
            {
                DocumentData = watermark
            })
            .Save();

        AssertPagesContainWatermark(result, expectedPageCount: 10, watermarkedPageCount: 5);
    }

    [Test]
    public void AttachmentFromContentTest()
    {
        var main = GenerateSampleDocumentData("attachment-memory-main", Colors.Red.Medium, 10);
        var content = System.Text.Encoding.UTF8.GetBytes("<invoice><total>100</total></invoice>");

        var result = DocumentOperation
            .LoadDocument(main)
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Key = "invoice.xml",
                Content = content
            })
            .Save();

        using var inspector = PdfInspector.Load(result);

        var attachment = inspector.Root.GetProperty("attachments").GetProperty("invoice.xml");
        Assert.That(attachment.GetProperty("preferredname").GetString(), Is.EqualTo("invoice.xml"));
    }

    [Test]
    public void AttachmentFromContentRequiresKeyOrAttachmentName()
    {
        var main = GenerateSampleDocumentData("attachment-name-input", Colors.Red.Medium, 1);

        Assert.Throws<ArgumentException>(() =>
        {
            DocumentOperation
                .LoadDocument(main)
                .AddAttachment(new DocumentOperation.DocumentAttachment
                {
                    Content = new byte[] { 1, 2, 3 }
                });
        });
    }

    [Test]
    public void AttachmentRequiresExactlyOneSource()
    {
        var main = GenerateSampleDocumentData("attachment-source-input", Colors.Red.Medium, 1);
        var operation = DocumentOperation.LoadDocument(main);

        Assert.Throws<ArgumentException>(() => operation.AddAttachment(new DocumentOperation.DocumentAttachment
        {
            Key = "data.bin"
        }));

        Assert.Throws<ArgumentException>(() => operation.AddAttachment(new DocumentOperation.DocumentAttachment
        {
            Key = "data.bin",
            FilePath = "data.bin",
            Content = new byte[] { 1, 2, 3 }
        }));
    }

    [Test]
    public void LayerRequiresExactlyOneSource()
    {
        var main = GenerateSampleDocumentData("layer-source-input", Colors.Red.Medium, 1);
        var operation = DocumentOperation.LoadDocument(main);

        Assert.Throws<ArgumentException>(() => operation.OverlayFile(new DocumentOperation.LayerConfiguration()));

        Assert.Throws<ArgumentException>(() => operation.OverlayFile(new DocumentOperation.LayerConfiguration
        {
            FilePath = "watermark.pdf",
            DocumentData = new byte[] { 1, 2, 3 }
        }));
    }

    [Test]
    public void EncryptionInMemoryRoundTrip()
    {
        var input = GenerateSampleDocumentData("encrypt-memory-input", Colors.Red.Medium, 5);

        var encrypted = DocumentOperation
            .LoadDocument(input)
            .Encrypt(new DocumentOperation.Encryption256Bit
            {
                UserPassword = "user_password",
                OwnerPassword = "owner_password"
            })
            .Save();

        using (var inspector = PdfInspector.Load(encrypted, password: "user_password"))
        {
            Assert.That(inspector.Root.GetProperty("encrypt").GetProperty("encrypted").GetBoolean(), Is.True);
            Assert.That(inspector.Pages.Count(), Is.EqualTo(5));
        }

        var decrypted = DocumentOperation
            .LoadDocument(encrypted, password: "owner_password")
            .Decrypt()
            .Save();

        using (var inspector = PdfInspector.Load(decrypted))
        {
            Assert.That(inspector.Root.GetProperty("encrypt").GetProperty("encrypted").GetBoolean(), Is.False);
        }
    }

    [Test]
    public void SaveToStreamTest()
    {
        var input = GenerateSampleDocumentData("stream-input", Colors.Red.Medium, 3);

        using var stream = new MemoryStream();

        DocumentOperation
            .LoadDocument(input)
            .TakePages("1-2")
            .Save(stream);

        using var inspector = PdfInspector.Load(stream.ToArray());
        Assert.That(inspector.Pages.Count(), Is.EqualTo(2));
    }

    [Test]
    public void SaveToStreamRequiresWritableStream()
    {
        var input = GenerateSampleDocumentData("stream-readonly-input", Colors.Red.Medium, 1);

        using var stream = new MemoryStream(new byte[] { 1, 2, 3 }, writable: false);

        Assert.Throws<ArgumentException>(() => DocumentOperation.LoadDocument(input).Save(stream));
    }

    /// <summary>
    /// When the destination stream fails, the operation is aborted and the stream exception
    /// is reported as the root cause, instead of a generic qpdf error.
    /// </summary>
    [Test]
    public void SaveToStreamSurfacesDestinationFailureAsRootCause()
    {
        var input = GenerateSampleDocumentData("stream-failing-input", Colors.Red.Medium, 1);

        var exception = Assert.Catch<Exception>(() => DocumentOperation.LoadDocument(input).Save(new FailingStream()));

        Assert.That(exception.Message, Does.Contain("could not write the output document"));
        Assert.That(exception.InnerException, Is.InstanceOf<IOException>());
    }

    [Test]
    public void MixedFileAndInMemoryInputsTest()
    {
        GenerateSampleDocument("mixed-input.pdf", Colors.Red.Medium, 3);
        var merged = GenerateSampleDocumentData("mixed-merged", Colors.Green.Medium, 5);

        var result = DocumentOperation
            .LoadFile("mixed-input.pdf")
            .MergeDocument(merged)
            .Save();

        using var inspector = PdfInspector.Load(result);
        Assert.That(inspector.Pages.Count(), Is.EqualTo(3 + 5));
    }

    [Test]
    public void InMemoryInputWithFileOutputTest()
    {
        var input = GenerateSampleDocumentData("memory-to-file-input", Colors.Red.Medium, 4);

        DocumentOperation
            .LoadDocument(input)
            .TakePages("1-2")
            .Save("operation-memory-to-file.pdf");

        using var inspector = PdfInspector.Load(File.ReadAllBytes("operation-memory-to-file.pdf"));
        Assert.That(inspector.Pages.Count(), Is.EqualTo(2));
    }

    /// <summary>
    /// qpdf error messages reference in-memory documents by their registered names,
    /// so failures point to the exact input that caused them.
    /// </summary>
    [Test]
    public void LoadCorruptedBinaryDataThrowsMeaningfulError()
    {
        var corrupted = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        var exception = Assert.Catch<Exception>(() => DocumentOperation.LoadDocument(corrupted).Save());

        Assert.That(exception.Message, Does.Contain("qpdf-buffer://input"));
    }

    [Test]
    public void LoadDocumentRejectsMissingInput()
    {
        Assert.Throws<ArgumentException>(() => DocumentOperation.LoadDocument(null!));
        Assert.Throws<ArgumentException>(() => DocumentOperation.LoadDocument(Array.Empty<byte>()));
    }

    private sealed class FailingStream : Stream
    {
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 0;
        public override long Position { get => 0; set { } }

        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new IOException("Simulated stream failure.");
    }

    #endregion

    #region Attachment Metadata

    /// <summary>
    /// The attachment entries are asserted on the document object graph rather than on the qpdf "attachments"
    /// summary section: the summary does not expose the relationship at all, and reports the modification date
    /// as the creation date.
    /// </summary>
    private static JsonElement GetAttachmentFileSpecification(PdfInspector inspector, string key)
    {
        var attachment = inspector.Root.GetProperty("attachments").GetProperty(key);
        return inspector.Resolve(attachment.GetProperty("filespec"));
    }

    private static JsonElement GetAttachmentEmbeddedFile(PdfInspector inspector, string key)
    {
        var fileSpecification = GetAttachmentFileSpecification(inspector, key);
        return inspector.Resolve(fileSpecification.GetProperty("/EF").GetProperty("/F")).GetProperty("dict");
    }

    [TestCase(DocumentOperation.DocumentAttachmentRelationship.Data, "/Data")]
    [TestCase(DocumentOperation.DocumentAttachmentRelationship.Source, "/Source")]
    [TestCase(DocumentOperation.DocumentAttachmentRelationship.Alternative, "/Alternative")]
    [TestCase(DocumentOperation.DocumentAttachmentRelationship.Supplement, "/Supplement")]
    [TestCase(DocumentOperation.DocumentAttachmentRelationship.Unspecified, "/Unspecified")]
    public void AttachmentRelationshipIsWrittenToDocument(DocumentOperation.DocumentAttachmentRelationship relationship, string expectedValue)
    {
        var input = GenerateSampleDocumentData("attachment-relationship-input", Colors.Red.Medium, 1);

        var result = DocumentOperation
            .LoadDocument(input)
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Key = "invoice.xml",
                Content = Encoding.UTF8.GetBytes("<invoice/>"),
                Relationship = relationship
            })
            .Save();

        using var inspector = PdfInspector.Load(result);
        var fileSpecification = GetAttachmentFileSpecification(inspector, "invoice.xml");

        Assert.That(fileSpecification.GetProperty("/AFRelationship").GetString(), Is.EqualTo(expectedValue));
    }

    /// <summary>
    /// When no relationship is specified, the attachment is marked as /Unspecified.
    /// The entry cannot simply be left out of the job: qpdf then applies its own default of /Supplement,
    /// which would silently declare every such attachment a supplement to the document.
    /// </summary>
    [Test]
    public void AttachmentWithoutRelationshipIsMarkedUnspecified()
    {
        var input = GenerateSampleDocumentData("attachment-default-relationship-input", Colors.Red.Medium, 1);

        var result = DocumentOperation
            .LoadDocument(input)
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Key = "invoice.xml",
                Content = Encoding.UTF8.GetBytes("<invoice/>")
            })
            .Save();

        using var inspector = PdfInspector.Load(result);
        var fileSpecification = GetAttachmentFileSpecification(inspector, "invoice.xml");

        Assert.That(fileSpecification.GetProperty("/AFRelationship").GetString(), Is.EqualTo("/Unspecified"));
    }

    [Test]
    public void AttachmentMetadataIsWrittenToDocument()
    {
        var input = GenerateSampleDocumentData("attachment-metadata-input", Colors.Red.Medium, 1);
        var content = Encoding.UTF8.GetBytes("<invoice><total>100</total></invoice>");

        var result = DocumentOperation
            .LoadDocument(input)
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Key = "invoice.xml",
                AttachmentName = "invoice-2026.xml",
                Content = content,
                Description = "Structured invoice data",
                MimeType = "text/xml",
                Relationship = DocumentOperation.DocumentAttachmentRelationship.Alternative,
                CreationDate = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc),
                ModificationDate = new DateTime(2026, 6, 7, 8, 9, 10, DateTimeKind.Utc)
            })
            .Save();

        using var inspector = PdfInspector.Load(result);

        var fileSpecification = GetAttachmentFileSpecification(inspector, "invoice.xml");
        Assert.That(fileSpecification.GetProperty("/AFRelationship").GetString(), Is.EqualTo("/Alternative"));
        Assert.That(fileSpecification.GetProperty("/Desc").GetString(), Is.EqualTo("u:Structured invoice data"));
        Assert.That(fileSpecification.GetProperty("/F").GetString(), Is.EqualTo("u:invoice-2026.xml"));

        var embeddedFile = GetAttachmentEmbeddedFile(inspector, "invoice.xml");
        Assert.That(embeddedFile.GetProperty("/Subtype").GetString(), Is.EqualTo("/text/xml"));

        var parameters = embeddedFile.GetProperty("/Params");
        Assert.That(parameters.GetProperty("/CreationDate").GetString(), Is.EqualTo("u:D:20260102030405Z"));
        Assert.That(parameters.GetProperty("/ModDate").GetString(), Is.EqualTo("u:D:20260607080910Z"));
        Assert.That(parameters.GetProperty("/Size").GetInt32(), Is.EqualTo(content.Length));

        // the checksum is computed by qpdf over the embedded bytes, and proves that the
        // in-memory content reached the document unchanged
        var checksum = inspector.Root
            .GetProperty("attachments").GetProperty("invoice.xml")
            .GetProperty("streams").GetProperty("/F")
            .GetProperty("checksum").GetString();

        Assert.That(checksum, Is.EqualTo(Convert.ToHexString(MD5.HashData(content)).ToLowerInvariant()));
    }

    /// <summary>
    /// The MIME type defaults to the one matching the file extension, for both attachment sources.
    /// The fallback applied when no extension is recognized is intentionally not asserted here,
    /// as in-memory attachments are identified by a label that need not carry one.
    /// </summary>
    [Test]
    public void AttachmentMimeTypeDefaultsToFileExtension()
    {
        var input = GenerateSampleDocumentData("attachment-mime-input", Colors.Red.Medium, 1);
        GenerateSampleDocument("attachment-mime-file.pdf", Colors.Green.Medium, 1);

        var result = DocumentOperation
            .LoadDocument(input)
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                Key = "invoice.xml",
                Content = Encoding.UTF8.GetBytes("<invoice/>")
            })
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                FilePath = "attachment-mime-file.pdf"
            })
            .Save();

        using var inspector = PdfInspector.Load(result);

        Assert.That(GetAttachmentEmbeddedFile(inspector, "invoice.xml").GetProperty("/Subtype").GetString(), Is.EqualTo("/text/xml"));
        Assert.That(GetAttachmentEmbeddedFile(inspector, "attachment-mime-file.pdf").GetProperty("/Subtype").GetString(), Is.EqualTo("/application/pdf"));
    }

    /// <summary>
    /// In-memory inputs are retained by the operation, so the same configuration can be saved repeatedly.
    /// </summary>
    [Test]
    public void OperationCanBeSavedMultipleTimes()
    {
        var input = GenerateSampleDocumentData("multiple-saves-input", Colors.Red.Medium, 5);

        var operation = DocumentOperation
            .LoadDocument(input)
            .TakePages("1-3");

        var fromMemory = operation.Save();

        using var stream = new MemoryStream();
        operation.Save(stream);

        operation.Save("operation-multiple-saves.pdf");

        var results = new[] { fromMemory, stream.ToArray(), File.ReadAllBytes("operation-multiple-saves.pdf") };

        foreach (var result in results)
        {
            using var inspector = PdfInspector.Load(result);
            Assert.That(inspector.Pages.Count(), Is.EqualTo(3));
        }
    }

    /// <summary>
    /// qpdf produces the output document in many small chunks, so a document of a realistic size
    /// exercises the output callback thousands of times.
    /// </summary>
    [Test]
    public void LargeDocumentIsStreamedCorrectly()
    {
        var input = GenerateSampleDocumentData("large-stream-input", Colors.Red.Medium, 100);

        using var stream = new MemoryStream();

        DocumentOperation
            .LoadDocument(input)
            .Save(stream);

        using var inspector = PdfInspector.Load(stream.ToArray());
        Assert.That(inspector.Pages.Count(), Is.EqualTo(100));
    }

    #endregion

    private void GenerateSampleDocument(string filePath, Color color, int length)
    {
        File.WriteAllBytes(filePath, GenerateSampleDocumentData(filePath, color, length));
    }

    private byte[] GenerateSampleDocumentData(string label, Color color, int length)
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
                                .Text($"{label}\npage {i}")
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
            .GeneratePdf();
    }
}