#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using QuestPDF.Qpdf;

namespace QuestPDF.UnitTests;

/// <summary>
/// Minimal helper for inspecting the internal structure of a generated PDF document in tests.
/// The document is converted to its JSON representation using the qpdf library bundled with QuestPDF.
/// The JSON mirrors the PDF object graph: dictionaries, arrays, indirect references ("12 0 R" strings),
/// and streams with inline base64 data. Reference: https://qpdf.readthedocs.io/en/stable/json.html
/// </summary>
internal sealed class PdfInspector : IDisposable
{
    private JsonDocument Document { get; }

    private PdfInspector(JsonDocument document)
    {
        Document = document;
    }

    public void Dispose()
    {
        Document.Dispose();
    }

    public static PdfInspector Load(byte[] pdfData)
    {
        var inputPath = Path.Combine(Path.GetTempPath(), $"questpdf-inspector-{Guid.NewGuid():N}.pdf");
        var outputPath = inputPath + ".json";

        try
        {
            File.WriteAllBytes(inputPath, pdfData);

            var job = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                ["inputFile"] = inputPath,
                ["outputFile"] = outputPath,
                ["json"] = "latest",
                ["jsonStreamData"] = "inline",
                ["decodeLevel"] = "generalized"
            });

            QpdfAPI.ExecuteJob(job);

            return new PdfInspector(JsonDocument.Parse(File.ReadAllBytes(outputPath)));
        }
        finally
        {
            File.Delete(inputPath);
            File.Delete(outputPath);
        }
    }

    /// <summary>Root of the qpdf JSON representation.</summary>
    public JsonElement Root => Document.RootElement;

    /// <summary>All indirect objects, keyed by "obj:N M R".</summary>
    private JsonElement Objects => Root.GetProperty("qpdf")[1];

    /// <summary>Page dictionaries, in document order.</summary>
    public IEnumerable<JsonElement> Pages => Root
        .GetProperty("pages")
        .EnumerateArray()
        .Select(page => Resolve(page.GetProperty("object")));

    /// <summary>
    /// If the element is an indirect reference (a "12 0 R" string), returns the object it points to:
    /// the value for plain objects, or a { "dict", "data" } element for streams.
    /// Any other element is returned unchanged, so direct and indirect values can be handled uniformly.
    /// </summary>
    public JsonElement Resolve(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.String || !Regex.IsMatch(element.GetString()!, @"^\d+ \d+ R$"))
            return element;

        var target = Objects.GetProperty($"obj:{element.GetString()}");
        return target.TryGetProperty("value", out var value) ? value : target.GetProperty("stream");
    }

    /// <summary>Returns the decoded content of a stream object (or of a reference to one).</summary>
    public byte[] GetStreamData(JsonElement stream)
    {
        return Resolve(stream).GetProperty("data").GetBytesFromBase64();
    }

    /// <summary>
    /// Returns the decoded content of a stream object as text.
    /// Latin1 maps every byte to the same character code, so binary sections survive unchanged
    /// and the textual parts (operators, hex strings, names) remain directly searchable.
    /// </summary>
    public string GetStreamText(JsonElement stream)
    {
        return Encoding.Latin1.GetString(GetStreamData(stream));
    }
}
