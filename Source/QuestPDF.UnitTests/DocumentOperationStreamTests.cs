using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

/// <summary>
/// This test suite focuses on executing various QPDF operations using stream-based APIs.
/// In most cases, it does not check the result but rather if any exception is thrown.
/// </summary>
public class DocumentOperationStreamTests
{
    public DocumentOperationStreamTests()
    {
        if (RuntimeInformation.RuntimeIdentifier == "linux-musl-x64")
            Assert.Ignore("The DocumentOperations functionality is not supported on Linux Musl, e.g. Alpine.");
    }
    
    [Test]
    public void LoadStreamTest()
    {
        var sourceBytes = GenerateSampleDocumentBytes("load-stream-source.pdf", Colors.Red.Medium, 5);
        
        using var sourceStream = new MemoryStream(sourceBytes);
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(sourceStream)
            .SaveToStream(outputStream);
            
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }
    
    [Test]
    public void MergeStreamTest()
    {
        var firstBytes = GenerateSampleDocumentBytes("merge-stream-first.pdf", Colors.Red.Medium, 3);
        var secondBytes = GenerateSampleDocumentBytes("merge-stream-second.pdf", Colors.Green.Medium, 5);
        var thirdBytes = GenerateSampleDocumentBytes("merge-stream-third.pdf", Colors.Blue.Medium, 7);
        
        using var firstStream = new MemoryStream(firstBytes);
        using var secondStream = new MemoryStream(secondBytes);
        using var thirdStream = new MemoryStream(thirdBytes);
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(firstStream)
            .MergeStream(secondStream)
            .MergeStream(thirdStream)
            .SaveToStream(outputStream);
            
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }
    
    [Test]
    public void OverlayStreamTest()
    {
        var mainBytes = GenerateSampleDocumentBytes("overlay-stream-main.pdf", Colors.Red.Medium, 10);
        var watermarkBytes = GenerateSampleDocumentBytes("overlay-stream-watermark.pdf", Colors.Green.Medium, 5);
        
        using var mainStream = new MemoryStream(mainBytes);
        using var watermarkStream = new MemoryStream(watermarkBytes);
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(mainStream)
            .OverlayStream(new DocumentOperation.LayerStreamConfiguration
            {
                Stream = watermarkStream
            })
            .SaveToStream(outputStream);
            
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }
    
    [Test]
    public void UnderlayStreamTest()
    {
        var mainBytes = GenerateSampleDocumentBytes("underlay-stream-main.pdf", Colors.Red.Medium, 10);
        var watermarkBytes = GenerateSampleDocumentBytes("underlay-stream-watermark.pdf", Colors.Green.Medium, 5);
        
        using var mainStream = new MemoryStream(mainBytes);
        using var watermarkStream = new MemoryStream(watermarkBytes);
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(mainStream)
            .UnderlayStream(new DocumentOperation.LayerStreamConfiguration
            {
                Stream = watermarkStream,
            })
            .SaveToStream(outputStream);
            
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }

    [Test]
    public void AttachmentStreamTest()
    {
        var mainBytes = GenerateSampleDocumentBytes("attachment-stream-main.pdf", Colors.Red.Medium, 10);
        var attachmentBytes = GenerateSampleDocumentBytes("attachment-stream-file.pdf", Colors.Green.Medium, 5);
        
        using var mainStream = new MemoryStream(mainBytes);
        using var attachmentStream = new MemoryStream(attachmentBytes);
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(mainStream)
            .AddAttachmentStream(new DocumentOperation.DocumentAttachmentStream
            {
                Stream = attachmentStream,
                AttachmentName = "attached-document.pdf"
            })
            .SaveToStream(outputStream);
            
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }
    
    [Test]
    public void MixedFileAndStreamOperationsTest()
    {
        // Create a file-based document
        GenerateSampleDocument("mixed-file-input.pdf", Colors.Red.Medium, 5);
        
        // Create stream-based documents
        var mergeBytes = GenerateSampleDocumentBytes("mixed-stream-merge.pdf", Colors.Green.Medium, 3);
        var overlayBytes = GenerateSampleDocumentBytes("mixed-stream-overlay.pdf", Colors.Blue.Medium, 2);
        
        using var mergeStream = new MemoryStream(mergeBytes);
        using var overlayStream = new MemoryStream(overlayBytes);
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadFile("mixed-file-input.pdf")
            .MergeStream(mergeStream)
            .OverlayStream(new DocumentOperation.LayerStreamConfiguration
            {
                Stream = overlayStream
            })
            .SaveToStream(outputStream);
            
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }
    
    [Test]
    public void LoadStreamWithPasswordTest()
    {
        // Create an encrypted PDF first
        var sourceBytes = GenerateSampleDocumentBytes("encrypted-stream-source.pdf", Colors.Red.Medium, 5);
        
        using var sourceStream = new MemoryStream(sourceBytes);
        using var encryptedStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(sourceStream)
            .Encrypt(new DocumentOperation.Encryption256Bit()
            {
                UserPassword = "user_password",
                OwnerPassword = "owner_password"
            })
            .SaveToStream(encryptedStream);
            
        // Now try to load the encrypted stream
        encryptedStream.Position = 0;
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(encryptedStream, "owner_password")
            .SaveToStream(outputStream);
            
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }
    
    private byte[] GenerateSampleDocumentBytes(string fileName, Color color, int length)
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
                                .TranslateX(horizontalTranslation)
                                .TranslateY(verticalTranslation)
                                .Width(width)
                                .Height(height)
                                .Background(color.WithAlpha(64))
                                .AlignCenter()
                                .AlignMiddle()
                                .Text($"{fileName}\npage {i}")
                                .FontColor(color)
                                .Bold()
                                .FontSize(16);
                        }
                    });
                });
            })
            .WithSettings(new DocumentSettings { PdfA = true })
            .GeneratePdf();
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
                                .TranslateX(horizontalTranslation)
                                .TranslateY(verticalTranslation)
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
            .WithSettings(new DocumentSettings { PdfA = true })
            .GeneratePdf(filePath);
    }
}