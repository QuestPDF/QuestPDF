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

    public static PdfInspector Load(byte[] pdfData, string? password = null)
    {
        // the input document is passed directly as an in-memory buffer;
        // the JSON representation must still go through a file, as qpdf does not support
        // in-memory output for the json mode
        var outputPath = Path.Combine(Path.GetTempPath(), $"questpdf-inspector-{Guid.NewGuid():N}.json");

        try
        {
            var job = new Dictionary<string, string>
            {
                ["inputFile"] = $"{QpdfAPI.BufferReferenceScheme}document",
                ["outputFile"] = outputPath,
                ["json"] = "latest",
                ["jsonStreamData"] = "inline",
                ["decodeLevel"] = "generalized"
            };

            if (password != null)
                job["password"] = password;

            var inputBuffers = new Dictionary<string, byte[]>
            {
                ["document"] = pdfData
            };

            QpdfAPI.ExecuteJob(JsonSerializer.Serialize(job), inputBuffers, outputBufferName: null, outputStream: null);

            return new PdfInspector(JsonDocument.Parse(File.ReadAllBytes(outputPath)));
        }
        finally
        {
            File.Delete(outputPath);
        }
    }

    /// <summary>Root of the qpdf JSON representation.</summary>
    public JsonElement Root => Document.RootElement;

    /// <summary>All indirect objects, keyed by "obj:N M R".</summary>
    private JsonElement Objects => Root.GetProperty("qpdf")[1];

    /// <summary>The trailer dictionary of the document.</summary>
    public JsonElement Trailer => Objects.GetProperty("trailer").GetProperty("value");

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

    /// <summary>
    /// Matches the lexical classes of a content stream: dictionary delimiters, hex strings,
    /// literal strings (with escapes), name objects, and bare keywords. Only keywords are operators;
    /// the other classes are consumed as opaque tokens so their content cannot produce false
    /// operator matches. Numbers are operands and fall through unmatched.
    /// </summary>
    private static readonly Regex ContentStreamToken = new(
        @"<<|>>|<[^>]*>|\((?:\\.|[^)\\])*\)|/[^\s\[\]<>/(){}%]+|(?<op>[A-Za-z'""][A-Za-z0-9*'""]*)");

    /// <summary>
    /// Extracts the sequence of operators (Tj, TJ, re, cm, ...) from content stream text,
    /// e.g. obtained via <see cref="GetStreamText"/>. The boolean and null keywords are operands
    /// and are excluded. Known limitation: binary data of inline images (between the ID and EI
    /// operators) is not skipped and may produce garbage tokens; QuestPDF never emits inline images.
    /// </summary>
    public static IEnumerable<string> GetContentStreamOperators(string stream)
    {
        return ContentStreamToken.Matches(stream)
            .Where(match => match.Groups["op"].Success)
            .Select(match => match.Groups["op"].Value)
            .Where(keyword => keyword is not ("true" or "false" or "null"));
    }
}
