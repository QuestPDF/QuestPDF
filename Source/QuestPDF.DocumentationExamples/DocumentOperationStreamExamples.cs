using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

[TestFixture]
public class DocumentOperationStreamExamples
{
    public DocumentOperationStreamExamples()
    {
        if (RuntimeInformation.RuntimeIdentifier == "linux-musl-x64")
            Assert.Ignore("The DocumentOperations functionality is not supported on Linux Musl, e.g. Alpine.");
    }
    
    [Test]
    public void LoadFromStreamAndSaveToStream()
    {
        // Generate a sample PDF as bytes
        var samplePdfBytes = Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .PaddingHorizontal(2, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Item().Text("Sample Document").FontSize(20).Bold();
                            column.Item().Text("This document was loaded from a stream and processed using DocumentOperation.").FontSize(12);
                        });
                });
            })
            .GeneratePdf();

        // Process the PDF using stream-based operations
        using var inputStream = new MemoryStream(samplePdfBytes);
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(inputStream)
            .Linearize() // Optimize for web
            .SaveToStream(outputStream);
            
        // The processed PDF is now in outputStream
        Assert.That(outputStream.Length, Is.GreaterThan(0));
        
        // You can write it to a file if needed
        File.WriteAllBytes("stream-processed-output.pdf", outputStream.ToArray());
    }

    [Test]
    public void MergeMultipleStreams()
    {
        // Create multiple PDF documents as byte arrays
        var doc1Bytes = CreateSampleDocument("Document 1", Colors.Red.Lighten3);
        var doc2Bytes = CreateSampleDocument("Document 2", Colors.Green.Lighten3);
        var doc3Bytes = CreateSampleDocument("Document 3", Colors.Blue.Lighten3);

        // Merge them using streams
        using var stream1 = new MemoryStream(doc1Bytes);
        using var stream2 = new MemoryStream(doc2Bytes);
        using var stream3 = new MemoryStream(doc3Bytes);
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(stream1)
            .MergeStream(stream2)
            .MergeStream(stream3)
            .SaveToStream(outputStream);
            
        // Save the merged document
        File.WriteAllBytes("merged-streams-output.pdf", outputStream.ToArray());
        
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }

    [Test]
    public void AddStreamBasedOverlay()
    {
        // Create main document and watermark
        var mainDocBytes = CreateSampleDocument("Main Document", Colors.Grey.Lighten4);
        var watermarkBytes = CreateWatermarkDocument();

        using var mainStream = new MemoryStream(mainDocBytes);
        using var watermarkStream = new MemoryStream(watermarkBytes);
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(mainStream)
            .OverlayStream(new DocumentOperation.LayerStreamConfiguration
            {
                Stream = watermarkStream
            })
            .SaveToStream(outputStream);
            
        File.WriteAllBytes("watermarked-output.pdf", outputStream.ToArray());
        
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }

    [Test]
    public void AddAttachmentFromStream()
    {
        // Create main document
        var mainDocBytes = CreateSampleDocument("Document with Attachment", Colors.Purple.Lighten3);
        
        // Create attachment content
        var attachmentContent = "This is the content of the attached text file.";
        var attachmentBytes = System.Text.Encoding.UTF8.GetBytes(attachmentContent);

        using var mainStream = new MemoryStream(mainDocBytes);
        using var attachmentStream = new MemoryStream(attachmentBytes);
        using var outputStream = new MemoryStream();
        
        DocumentOperation
            .LoadStream(mainStream)
            .AddAttachmentStream(new DocumentOperation.DocumentAttachmentStream
            {
                Stream = attachmentStream,
                AttachmentName = "readme.txt",
                Description = "Important information about this document",
                MimeType = "text/plain"
            })
            .SaveToStream(outputStream);
            
        File.WriteAllBytes("document-with-attachment.pdf", outputStream.ToArray());
        
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }

    [Test]
    public void MixFileAndStreamOperations()
    {
        // Create a file-based document first
        var filePdfBytes = CreateSampleDocument("File-based Document", Colors.Orange.Lighten3);
        File.WriteAllBytes("file-document.pdf", filePdfBytes);

        // Create stream-based content
        var streamPdfBytes = CreateSampleDocument("Stream-based Document", Colors.Teal.Lighten3);

        using var streamInput = new MemoryStream(streamPdfBytes);
        using var outputStream = new MemoryStream();
        
        // Mix file and stream operations
        DocumentOperation
            .LoadFile("file-document.pdf")  // Load from file
            .MergeStream(streamInput)       // Merge from stream
            .SaveToStream(outputStream);    // Save to stream
            
        File.WriteAllBytes("mixed-operations-output.pdf", outputStream.ToArray());
        
        Assert.That(outputStream.Length, Is.GreaterThan(0));
    }

    private byte[] CreateSampleDocument(string title, Color color)
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.PageColor(color);
                    page.Content()
                        .PaddingVertical(2, Unit.Centimetre)
                        .PaddingHorizontal(2, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Item().Text(title).FontSize(24).Bold().FontColor(Colors.White);
                            column.Item().PaddingTop(1, Unit.Centimetre);
                            column.Item().Text("This is a sample document created for stream-based operations demonstration.")
                                .FontSize(12).FontColor(Colors.White);
                        });
                });
            })
            .GeneratePdf();
    }

    private byte[] CreateWatermarkDocument()
    {
        return Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.Transparent);
                    page.Content()
                        .AlignCenter()
                        .AlignMiddle()
                        .Rotate(-45)
                        .Text("CONFIDENTIAL")
                        .FontSize(72)
                        .FontColor(Colors.Red.Darken2)
                        .Bold();
                });
            })
            .GeneratePdf();
    }
}