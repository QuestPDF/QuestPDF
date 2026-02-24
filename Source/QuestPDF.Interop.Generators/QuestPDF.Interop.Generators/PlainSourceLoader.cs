using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

public class PlainSourceLoader(string resourceName) : IInteropSourceGenerator
{
    public string GenerateCode(Compilation compilation, string language)
    {
        return LoadResource($"{language}.{resourceName}");
    }

    private static string LoadResource(string resourceName)
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"QuestPDF.Interop.Generators.Templates.{resourceName}.liquid");

        if (stream == null)
            return string.Empty;

        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }
}
