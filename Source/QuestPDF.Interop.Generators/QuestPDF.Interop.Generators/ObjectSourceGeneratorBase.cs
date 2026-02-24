using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using QuestPDF.Interop.Generators.Languages;

namespace QuestPDF.Interop.Generators;

internal abstract class ObjectSourceGeneratorBase : IInteropSourceGenerator
{
    public string? InheritFrom { get; set; }

    protected abstract IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation);
    protected abstract INamedTypeSymbol GetTargetType(Compilation compilation);

    public string GenerateCSharpCode(Compilation compilation)
    {
        var builder = new CSharpNativeExportBuilder(GetTargetType(compilation));
        var templateModel = builder.BuildTemplateModel(GetTargetMethods(compilation));
        return TemplateManager.RenderTemplate("CSharp.NativeInteropMethod", templateModel);
    }

    public string GeneratePythonCode(Compilation compilation)
    {
        return GenerateCode(compilation, "Python", new PythonLanguageProvider());
    }

    public string GenerateTypeScriptCode(Compilation compilation)
    {
        return GenerateCode(compilation, "TypeScript", new TypeScriptLanguageProvider());
    }

    public string GenerateKotlinCode(Compilation compilation)
    {
        return GenerateCode(compilation, "Kotlin", new KotlinLanguageProvider());
    }

    private string GenerateCode(Compilation compilation, string prefix, ILanguageProvider languageProvider)
    {
        var targetType = GetTargetType(compilation);
        var methods = GetTargetMethods(compilation).ToList();
        var overloads = methods.ComputeOverloads();
        var className = targetType.GetGeneratedClassName();

        var customDefinitions = TryLoadingCustomContent($"{prefix}.{className}.Object.Defs");
        var customInit = TryLoadingCustomContent($"{prefix}.{className}.Object.Init");
        var customClass = TryLoadingCustomContent($"{prefix}.{className}.Object.Class");

        var templateModel = languageProvider.BuildClassTemplateModel(
            targetType, methods, overloads, InheritFrom,
            customDefinitions, customInit, customClass);

        return TemplateManager.RenderTemplate($"{prefix}.Object", templateModel);
    }

    private string TryLoadingCustomContent(string id)
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"QuestPDF.Interop.Generators.Templates.{id}.liquid");

        if (stream == null)
            return string.Empty;

        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }
}
