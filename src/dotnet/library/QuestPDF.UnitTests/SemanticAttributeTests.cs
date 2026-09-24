using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

public class SemanticAttributeTests
{
    [Test]
    [NonParallelizable]
    public void GeneratingDocumentsConcurrentlyMatchSerialReference()
    {
        var reference = NormalizeGenerationOutput(RenderDocument());

        var documents = Enumerable
            .Range(0, 100)
            .AsParallel()
            .WithDegreeOfParallelism(16)
            .Select(_ => RenderDocument())
            .ToList();
            
        var differentDocuments = documents
            .Select(NormalizeGenerationOutput)
            .Count(document => document != reference);

        Assert.That(differentDocuments, Is.Zero);
    }

    private static byte[] RenderDocument()
    {
        GC.Collect();
        
        return Document
            .Create(document => document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);

                page.Content().SemanticTable().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                        columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Number");
                        header.Cell().Text("Square");
                        header.Cell().Text("Cube");
                    });

                    foreach (var i in Enumerable.Range(1, 25))
                    {
                        table.Cell().Text(i.ToString());
                        table.Cell().Text((i * i).ToString());
                        table.Cell().Text((i * i * i).ToString());
                    }
                });
            }))
            .WithMetadata(new DocumentMetadata
            {
                CreationDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                ModifiedDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            })
            .WithSettings(new DocumentSettings
            {
                PDFA_Conformance = PDFA_Conformance.PDFA_3A,
                PDFUA_Conformance = PDFUA_Conformance.PDFUA_1
            })
            .GeneratePdf();
    }

    private static string NormalizeGenerationOutput(byte[] data)
    {
        // PDF/UA documents get a random identifier: in the XMP metadata, and in the trailer (as a hex or literal string)
        var text = Encoding.Latin1.GetString(data);
        
        text = Regex.Replace(text, "uuid:[0-9a-f-]{36}", "uuid:");
        text = Regex.Replace(text, @"/ID \[.*\]>>\nstartxref", "/ID []>>\nstartxref");
        
        return text;
    }
}
