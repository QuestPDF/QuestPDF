#nullable enable

using System.Linq;

namespace QuestPDF.UnitTests.TestEngine;

/// <summary>
/// Reads named destinations (e.g. created by sections) from a generated PDF document.
/// </summary>
internal static class NamedDestination
{
    /// <summary>Returns the number of the page the destination points to, or null when the document does not define it.</summary>
    public static int? GetPageNumber(byte[] pdfData, string name)
    {
        using var pdf = PdfInspector.Load(pdfData);

        var root = pdf.Resolve(pdf.Trailer.GetProperty("/Root"));

        if (!root.TryGetProperty("/Dests", out var destinations))
            return null;

        if (!pdf.Resolve(destinations).TryGetProperty($"/{name}", out var destination))
            return null;

        var pageReferences = pdf.Root
            .GetProperty("pages")
            .EnumerateArray()
            .Select(page => page.GetProperty("object").GetString())
            .ToList();

        // a named destination is an array: [page reference, /XYZ, left, top, zoom]
        return pageReferences.IndexOf(pdf.Resolve(destination)[0].GetString()) + 1;
    }
}
