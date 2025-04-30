using System;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

/// <summary>
/// This test suite focus on executing various QPDF operations.
/// In most cases, it does not check the result but rather if any exception is thrown.
/// </summary>
public class DocumentOperationTests
{
    public DocumentOperationTests()
    {
        if (RuntimeInformation.RuntimeIdentifier == "linux-musl-x64")
            Assert.Ignore("The DocumentOperations functionality is not supported on Linux Musl, e.g. Alpine.");
    }
    
    [Test]
    public void TakePages()
    {
        GenerateSampleDocument("take-input.pdf", Colors.Red.Medium, 10);
        
        DocumentOperation
            .LoadFile("take-input.pdf")
            .TakePages("2-5")
            .Save("operation-take.pdf");
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
    }
    
    [Test]
    public void LinearizeTest()
    {
        GenerateSampleDocument("linearize-input.pdf", Colors.Red.Medium, 10);
        
        DocumentOperation
            .LoadFile("linearize-input.pdf")
            .Linearize()
            .Save("operation-linearize.pdf");
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