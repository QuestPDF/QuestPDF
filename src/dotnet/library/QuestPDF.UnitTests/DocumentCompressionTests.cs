using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

public class DocumentCompressionTests
{
    [Test]
    public void Test()
    {
        var document = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                
                page.Content()
                    .Column(column =>
                    {
                        foreach (var i in Enumerable.Range(0, 10))
                        {
                            column.Item().Text($"{i}: {Placeholders.LoremIpsum()}");
                        }
                    });
            });
        });

        // warmup cache
        document.GeneratePdf();

        var withoutCompression = MeasureDocumentSizeAndGenerationTime(false);
        var withCompression = MeasureDocumentSizeAndGenerationTime(true);
        
        var sizeRatio = withoutCompression.documentSize / (float)withCompression.documentSize;
        Assert.That(sizeRatio, Is.GreaterThan(5));

        var generationTimeRatio = withCompression.generationTime / (float)withoutCompression.generationTime;
        Assert.That(generationTimeRatio, Is.LessThan(2));
        
        (int documentSize, float generationTime) MeasureDocumentSizeAndGenerationTime(bool compress)
        {
            var stopwatch = new Stopwatch();
            
            stopwatch.Restart();
            
            var documentSize = document
                .WithSettings(new DocumentSettings { CompressDocument = compress })
                .GeneratePdf()
                .Length;
            
            stopwatch.Stop();
            
            return (documentSize, stopwatch.ElapsedMilliseconds);
        }
    }
}