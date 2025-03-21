using System.Runtime.InteropServices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.DocumentationExamples;

[TestFixture]
public class DocumentOperationExamples
{
    public DocumentOperationExamples()
    {
        if (RuntimeInformation.RuntimeIdentifier == "linux-musl-x64")
            Assert.Ignore("The DocumentOperations functionality is not supported on Linux Musl, e.g. Alpine.");
    }
    
    [Test]
    public void MergeFiles()
    {
        const string prefix = "document-operation-merge";
        
        GenerateSampleDocument($"{prefix}-source-red.pdf", Colors.Red.Lighten3, 2);
        GenerateSampleDocument($"{prefix}-source-green.pdf", Colors.Green.Lighten3, 3);
        GenerateSampleDocument($"{prefix}-source-blue.pdf", Colors.Blue.Lighten3, 5);
        
        DocumentOperation
            .LoadFile($"{prefix}-source-red.pdf")
            .MergeFile($"{prefix}-source-green.pdf")
            .MergeFile($"{prefix}-source-blue.pdf")
            .Save($"{prefix}-result.pdf");
    }
    
    [Test]
    public void SelectEvenPages()
    {
        const string prefix = "document-operation-select-even-pages";
        
        GenerateSampleDocument($"{prefix}-source.pdf", Colors.Indigo.Lighten3, 11);
        
        DocumentOperation
            .LoadFile($"{prefix}-source.pdf")
            .TakePages("1-z:even")
            .Save($"{prefix}-result.pdf");
    }

    [Test]
    public void Encrypt()
    {
        const string prefix = "document-operation-encrypt";
        
        GenerateSampleDocument($"{prefix}-source.pdf", Colors.Orange.Lighten3, 7);
        
        DocumentOperation
            .LoadFile($"{prefix}-source.pdf")
            .Encrypt(new DocumentOperation.Encryption256Bit()
            {
                UserPassword = "user-password",
                OwnerPassword = "owner-password",
                AllowContentExtraction = false,
                AllowPrinting = false
            })
            .Save($"{prefix}-result.pdf");
    }
    
    [Test]
    public void AddAttachment()
    {
        const string prefix = "document-operation-add-attachment";
        
        GenerateSampleDocument($"{prefix}-source.pdf", Colors.Cyan.Lighten3, 7);
        File.WriteAllText($"{prefix}-content.txt", "Hello, World!");
        
        DocumentOperation
            .LoadFile($"{prefix}-source.pdf")
            .AddAttachment(new DocumentOperation.DocumentAttachment
            {
                FilePath = $"{prefix}-content.txt",
                AttachmentName = "Attached message"
            })
            .Save($"{prefix}-result.pdf");
    }
    
    [Test]
    public void Overlay()
    {
        const string prefix = "document-operation-overlay";
        
        GenerateSampleDocument($"{prefix}-source.pdf", Colors.Cyan.Lighten3, 7);
        
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.Transparent);
                    
                    page.Content().Column(column =>
                    {
                        foreach (var i in Enumerable.Range(0, 6))
                            column.Item().PageBreak();
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(24).Bold().FontColor(Colors.White));
                        text.Span("Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
                });
            })
            .GeneratePdf($"{prefix}-content.pdf");
        
        DocumentOperation
            .LoadFile($"{prefix}-source.pdf")
            .OverlayFile(new DocumentOperation.LayerConfiguration
            {
                FilePath = $"{prefix}-content.pdf"
            })
            .Save($"{prefix}-result.pdf");
    }
    
    private void GenerateSampleDocument(string fileName, Color pageColor, int numberOfPages)
    {
        Document
            .Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(pageColor);
                    
                    page.Content().Column(column =>
                    {
                        foreach (var pageNumber in Enumerable.Range(1, numberOfPages))
                        {
                            column.Item()
                                .Extend()
                                .AlignCenter().AlignMiddle()
                                .Text($"{pageNumber}")
                                .FontSize(256)
                                .FontColor(Colors.White)
                                .Bold();
                            
                            if (pageNumber != numberOfPages)
                                column.Item().PageBreak();
                        }
                    });
                });
            })
            .GeneratePdf(fileName);
    }
}