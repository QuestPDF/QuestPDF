using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

public class PlainSourceLoader(string resourceName) : IInteropSourceGenerator
{
    public string GenerateCSharpCode(Compilation compilation)
    {
        return LoadResource($"CSharp.{resourceName}");
    }

    public string GeneratePythonCode(Compilation compilation)
    {
        return LoadResource($"Python.{resourceName}");
    }

    public string GenerateTypeScriptCode(Compilation compilation)
    {
        return LoadResource($"TypeScript.{resourceName}");
    }

    public string GenerateKotlinCode(Compilation compilation)
    {
        return LoadResource($"Kotlin.{resourceName}");
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