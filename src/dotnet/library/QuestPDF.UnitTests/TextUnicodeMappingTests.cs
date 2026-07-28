using System;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.UnitTests;

/// <summary>
/// Regression test for concurrent PDF generation:
/// https://github.com/QuestPDF/QuestPDF/issues/1448
/// https://github.com/QuestPDF/QuestPDF/issues/1447
///
/// Document content is fixed, so generation is deterministic: a serial render defines the
/// expected ToUnicode CMaps, and every concurrently generated document must carry identical
/// ones. Any corruption shape - zeroed destinations, missing entries, malformed sections,
/// a missing stream - breaks the equality.
/// </summary>
public class TextUnicodeMappingTests
{
    private const string TestedCharacters =
        " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

    /// <summary>
    /// Generated once from a verified serial render. The source codes are the typeface's native
    /// glyph identifiers; all three weights carry the same mapping because Lato's weights share
    /// one glyph layout and render the same text. A Skia or font update that changes CMap
    /// emission fails this test on purpose: review the new mapping and update the snapshot
    /// consciously.
    /// </summary>
    private const string ExpectedToUnicodeCmap =
        """
        /CIDInit /ProcSet findresource begin
        12 dict begin
        begincmap
        /CIDSystemInfo
        <<  /Registry (Adobe)
        /Ordering (UCS)
        /Supplement 0
        >> def
        /CMapName /Adobe-Identity-UCS def
        /CMapType 2 def
        1 begincodespacerange
        <0000> <FFFF>
        endcodespacerange
        57 beginbfchar
        <0002> <0020>
        <0003> <0041>
        <0009> <0044>
        <000B> <0045>
        <0011> <0049>
        <001B> <004F>
        <001E> <0050>
        <0024> <0054>
        <0026> <0055>
        <002D> <0061>
        <0037> <0064>
        <003B> <0065>
        <003D> <0066>
        <005D> <0067>
        <005F> <0068>
        <0062> <0069>
        <0066> <006A>
        <0068> <006B>
        <006F> <006F>
        <0072> <0070>
        <007C> <0074>
        <0086> <0075>
        <00AA> <0060>
        <0134> <0021>
        <0135> <003F>
        <0137> <002C>
        <0138> <003B>
        <0139> <003A>
        <013A> <002E>
        <0146> <002F>
        <0147> <007C>
        <0149> <005C>
        <014A> <002D>
        <014D> <005F>
        <0152> <005B>
        <0153> <005D>
        <0154> <007B>
        <0155> <007D>
        <0156> <002A>
        <015B> <005E>
        <015C> <007E>
        <015D> <0027>
        <015E> <0022>
        <015F> <0026>
        <0160> <0040>
        <016A> <0024>
        <0172> <0023>
        <0195> <0037>
        <0196> <0039>
        <01B3> <002B>
        <01B7> <003D>
        <01BA> <003C>
        <01BB> <003E>
        <0474> <006C>
        <0475> <0036>
        <0476> <0038>
        <0477> <0025>
        endbfchar
        12 beginbfrange
        <0006> <0007> <0042>
        <000D> <000F> <0046>
        <0013> <0015> <004A>
        <0018> <0019> <004D>
        <0020> <0022> <0051>
        <0028> <002C> <0056>
        <0030> <0031> <0062>
        <006C> <006D> <006D>
        <0074> <0076> <0071>
        <0088> <008C> <0076>
        <0150> <0151> <0028>
        <018F> <0194> <0030>
        endbfrange
        endcmap
        CMapName currentdict /CMap defineresource pop
        end
        end
        """;

    [Test]
    public void FontSubsetsCarryTheExpectedToUnicodeCmap()
    {
        using var pdf = PdfInspector.Load(RenderDocument());

        var maps = pdf.Pages
            .SelectMany(page => page.GetProperty("/Resources").GetProperty("/Font").EnumerateObject())
            .Select(font => (font.Name, Object: pdf.Resolve(font.Value)))
            .Select(x =>
                x.Object.TryGetProperty("/ToUnicode", out var unicode)
                    ? pdf.GetStreamText(unicode).ReplaceLineEndings("\n")
                    : null)
            .Where(x => x != null)
            .ToList();

        Assert.That(maps, Has.Count.EqualTo(3));
        Assert.That(maps, Is.All.EqualTo(ExpectedToUnicodeCmap.ReplaceLineEndings("\n")));
    }

    [Test]
    [NonParallelizable]
    public void GeneratingDocumentsConcurrentlyMatchSerialReference()
    {
        var reference = RenderDocument();
        
        Enumerable
            .Range(0, 1_000)
            .AsParallel()
            .WithDegreeOfParallelism(16)
            .Select(_ => RenderDocument())
            .ToList() // render everything first so validation does not throttle the contention
            .AsParallel()
            .ForAll(document => Assert.That(document, Is.EqualTo(reference)));
    }

    private static byte[] RenderDocument()
    {
        return Document
            .Create(document => document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Content().Column(column =>
                {
                    column.Item().Text(TestedCharacters).Bold();
                    column.Item().Text(TestedCharacters).SemiBold();
                    column.Item().Text(TestedCharacters);
                });
            }))
            .WithMetadata(new DocumentMetadata
            {
                CreationDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                ModifiedDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            })
            .GeneratePdf();
    }
}