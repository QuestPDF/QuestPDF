using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
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
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(100);
                        });

                        foreach (var y in Enumerable.Range(1, 1_00))
                        {
                            foreach (var x in Enumerable.Range(1, 4))
                            {
                                table
                                    .Cell()
                                    .Padding(5)
                                    .Border(1)
                                    .Background(Placeholders.BackgroundColor())
                                    .Padding(5)
                                    .Text($"f({y}, {x}) = '{Placeholders.Sentence()}'");
                            }
                        
                            table
                                .Cell()
                                .Padding(5)
                                .AspectRatio(2f)
                                .Image(Placeholders.Image);
                        }
                    });
            });
        });

        var withoutCompression = MeasureDocumentSizeAndGenerationTime(false);
        var withCompression = MeasureDocumentSizeAndGenerationTime(true);
        
        var sizeRatio = withoutCompression.documentSize / (float)withCompression.documentSize;
        sizeRatio.Should().BeGreaterThan(3);

        Math.Abs(withCompression.generationTime - withoutCompression.generationTime).Should().BeLessThan(200);
        
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