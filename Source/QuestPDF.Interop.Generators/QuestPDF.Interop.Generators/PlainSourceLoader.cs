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
        return TemplateManager.TryLoadRawTemplate(resourceName);
    }
}