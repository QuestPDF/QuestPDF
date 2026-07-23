using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using UglyToad.PdfPig;

namespace QuestPDF.ReportSample;

/// <summary>
/// Regression tests for concurrent PDF generation:
/// https://github.com/QuestPDF/QuestPDF/issues/1448
/// https://github.com/QuestPDF/QuestPDF/issues/1447#issuecomment-5059644181
///
/// Concurrent GeneratePdf() calls used to intermittently corrupt the embedded font subset's
/// ToUnicode CMap. The page painted correctly, but text extracted as U+0000 characters,
/// silently breaking copy/paste, search, screen readers, and archival text extraction.
/// The corruption rate was reported as 2-4% of overlapping renders, so this test
/// performs a large number of them to achieve a high detection probability.
/// </summary>
public class ConcurrentPdfGenerationTests
{
    private const int RenderCount = 100_000;
    private const string Sentinel = "The quick brown fox jumps over the lazy dog";

    [SetUp]
    public void SetUp()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    [Test]
    [NonParallelizable]
    public void GeneratePdfConcurrently_TextLayerIsNotCorrupted()
    {
        // a serial warm-up render proves the document itself is valid,
        // so any failure below implicates concurrency rather than the document content
        var warmUpError = ValidatePdf(RenderDocument(seed: 34567), seed: 34567);
        Assert.That(warmUpError, Is.Null, $"Serial warm-up render failed: {warmUpError}");

        var documents = new List<byte[]>();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = 16
        };

        Parallel.For(0, RenderCount, options, seed =>
        {
            var document = RenderDocument(seed);
            
            lock (documents)
                documents.Add(document);
        });

        var failures = documents
            .Select(ValidatePdfLight)
            .Where(x => x != null)
            .ToList();
        
        var totalSize = documents.Sum(x => x.Length);

        Assert.That(failures, Is.Empty,
            $"{failures.Count}/{RenderCount} parallel renders produced a corrupted document:\n{string.Join("\n", failures.Take(100).Order())}");
    }

    private static string SeedMarker(int seed) => $"SEED-MARKER-{seed:D5}";

    private static byte[] RenderDocument(int seed)
    {
        return Document
            .Create(document => document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.Header().Text("Concurrency repro document").FontSize(14).SemiBold();
                page.Content().Column(column =>
                {
                    column.Item().Text(SeedMarker(seed)).Bold();
                    column.Item().Text(Sentinel);
                    column.Item().Text("Second paragraph, regular weight, plain ASCII.");
                });
            }))
            .GeneratePdf();
    }

    private static string? ValidatePdfLight(byte[] data)
    {
        string text;

        try
        {
            using var document = PdfDocument.Open(data);
            text = string.Concat(document.GetPages().Select(page => page.Text));
        }
        catch (Exception exception)
        {
            return $"Cannot parse the document: {exception.GetType().Name}: {exception.Message}";
        }
        
        if (text.Contains('\0'))
            return $"Extracted text contains U+0000 characters (corrupted ToUnicode CMap).";

        if (!text.Contains(Sentinel, StringComparison.Ordinal))
            return $"Expected text '{Sentinel}' not found in extracted content.";

        return null;
    }
    
    private static string? ValidatePdf(byte[] data, int seed)
    {
        string text;

        try
        {
            using var document = PdfDocument.Open(data);
            text = string.Concat(document.GetPages().Select(page => page.Text));
        }
        catch (Exception exception)
        {
            return $"[seed={seed}] Cannot parse the document: {exception.GetType().Name}: {exception.Message}";
        }

        // a corrupted ToUnicode CMap maps glyphs to U+0000
        if (text.Contains('\0'))
            return $"[seed={seed}] Extracted text contains U+0000 characters (corrupted ToUnicode CMap).";

        if (!text.Contains(Sentinel, StringComparison.Ordinal))
            return $"[seed={seed}] Expected text '{Sentinel}' not found in extracted content.";

        if (!text.Contains(SeedMarker(seed), StringComparison.Ordinal))
            return $"[seed={seed}] Expected text '{SeedMarker(seed)}' not found in extracted content.";

        var leakedMarker = Regex
            .Matches(text, @"SEED-MARKER-\d{5}")
            .Select(match => match.Value)
            .FirstOrDefault(marker => marker != SeedMarker(seed));

        if (leakedMarker != null)
            return $"[seed={seed}] Found text '{leakedMarker}' that belongs to a different document.";

        return null;
    }
}
