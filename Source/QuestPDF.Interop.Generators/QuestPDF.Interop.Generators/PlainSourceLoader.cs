using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

public class PlainSourceLoader(string resourceName) : IInteropSourceGenerator
{
    public string GenerateCode(Compilation compilation, string language)
    {
        return TemplateManager.LoadEmbeddedContent($"{language}.{resourceName}");
    }
}
