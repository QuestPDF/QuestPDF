using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace QuestPDF.Qpdf;

static class MimeHelper
{
    public static readonly IReadOnlyDictionary<string, string> FileExtensionToMimeConversionTable = LoadMimeMapping();

    private static IReadOnlyDictionary<string, string> LoadMimeMapping()
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("QuestPDF.Resources.MimeTypes.csv");
        
        using var streamReader = new StreamReader(stream);
        var text = streamReader.ReadToEnd();

        return text
            .Split('\n')
            .Select(x => x.Split(','))
            .ToDictionary(x => x.First(), x => x.Last());
    }
}