#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace QuestPDF.UnitTests.TestEngine;

/// <summary>
/// A link annotation read from a generated PDF document.
/// The rectangle uses PDF coordinates: points measured from the bottom-left corner of the page.
/// </summary>
internal sealed record LinkAnnotation(float Left, float Bottom, float Right, float Top, string? Url, string? Destination)
{
    /// <summary>Returns the link annotations of every page, in document order.</summary>
    public static List<List<LinkAnnotation>> ReadAll(byte[] pdfData)
    {
        using var pdf = PdfInspector.Load(pdfData);

        return pdf.Pages
            .Select(page => ReadPage(pdf, page))
            .ToList();
    }

    private static List<LinkAnnotation> ReadPage(PdfInspector pdf, JsonElement page)
    {
        if (!page.TryGetProperty("/Annots", out var annotations))
            return [];

        return pdf
            .Resolve(annotations)
            .EnumerateArray()
            .Select(annotation => Read(pdf, pdf.Resolve(annotation)))
            .ToList();
    }

    private static LinkAnnotation Read(PdfInspector pdf, JsonElement annotation)
    {
        var rectangle = pdf
            .Resolve(annotation.GetProperty("/Rect"))
            .EnumerateArray()
            .Select(x => MathF.Round(x.GetSingle(), 2))
            .ToArray();

        var url = annotation.TryGetProperty("/A", out var action)
            ? GetText(pdf.Resolve(action).GetProperty("/URI"))
            : null;

        // named destinations are PDF names, e.g. "/chapter"
        var destination = annotation.TryGetProperty("/Dest", out var name)
            ? name.GetString()!.TrimStart('/')
            : null;

        return new LinkAnnotation(rectangle[0], rectangle[1], rectangle[2], rectangle[3], url, destination);
    }

    // qpdf prefixes text strings with "u:"
    private static string GetText(JsonElement element)
    {
        return element.GetString()!.Substring("u:".Length);
    }
}
