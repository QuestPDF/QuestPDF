#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace QuestPDF.UnitTests;

/// <summary>
/// Regression tests for https://github.com/QuestPDF/QuestPDF/issues/1494
///
/// The text shaper hides line-break hint characters by replacing each of them with the font's space glyph of zero advance.
/// When that glyph reaches the PDF, it is invisible, but text extraction reads it as U+0020,
/// so search, copy-paste and screen readers see the word split in two ("light house").
///
/// The documents are read with PdfPig, an independent PDF library, the same way a PDF viewer extracts text.
/// </summary>
public class TextExtractionTests
{
    [TestCase("soft hyphen", '­')]
    [TestCase("zero-width space", '​')]
    [TestCase("word joiner", '⁠')]
    [TestCase("byte order mark", '﻿')]
    public void LineBreakHintInsideLineIsNotExtracted(string hintName, char hint)
    {
        var letters = RenderAndExtractLetters(400, $"The old light{hint}house stood alone.");

        Assert.That(CountLines(letters), Is.EqualTo(1));
        Assert.That(GetText(letters), Is.EqualTo("The old lighthouse stood alone."), hintName);
    }

    [Test]
    public void SoftHyphenThatBreaksLineIsExtractedAsHyphenOnly()
    {
        var letters = RenderAndExtractLetters(95, "The old light­house");

        Assert.That(CountLines(letters), Is.EqualTo(2));
        Assert.That(GetText(letters), Is.EqualTo("The old light-house"));
    }

    [Test]
    public void ZeroWidthSpaceThatBreaksLineIsNotExtracted()
    {
        var letters = RenderAndExtractLetters(95, "The old light​house");

        Assert.That(CountLines(letters), Is.EqualTo(2));
        Assert.That(GetText(letters), Is.EqualTo("The old lighthouse"));
    }

    [TestCase("soft hyphen", '­')]
    [TestCase("zero-width space", '​')]
    public void LettersAroundLineBreakHintDoNotShareStartPosition(string hintName, char hint)
    {
        // the hidden space glyph has no advance, so it used to start exactly where the following letter starts;
        // positional extractors then reported it as overlapping that letter
        var letters = RenderAndExtractLetters(400, $"The old light{hint}house stood alone.");

        var startPositions = letters
            .Select(x => Math.Round(x.StartBaseLine.X, 2))
            .ToList();

        Assert.That(startPositions, Is.Unique, hintName);
    }

    private static string GetText(IEnumerable<Letter> letters)
    {
        return string.Concat(letters.Select(x => x.Value));
    }

    private static int CountLines(IEnumerable<Letter> letters)
    {
        return letters
            .Select(x => Math.Round(x.StartBaseLine.Y))
            .Distinct()
            .Count();
    }

    private static IReadOnlyList<Letter> RenderAndExtractLetters(float width, string text)
    {
        var documentBytes = Document
            .Create(document => document.Page(page =>
            {
                page.Size(500, 200);
                page.Margin(20);
                page.Content().Width(width).Text(text).FontSize(14);
            }))
            .GeneratePdf();

        using var pdf = PdfDocument.Open(documentBytes);
        return pdf.GetPage(1).Letters.ToList();
    }
}
