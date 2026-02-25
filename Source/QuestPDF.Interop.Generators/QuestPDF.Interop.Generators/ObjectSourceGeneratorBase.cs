using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using QuestPDF.Interop.Generators.Languages;

namespace QuestPDF.Interop.Generators;

internal abstract class ObjectSourceGeneratorBase(Type targetType) : IInteropSourceGenerator
{
    public string? InheritFrom { get; init; }
    public ICollection<string> ExcludeMembers { get; init; } = [];
    protected Type TargetClrType => targetType;

    protected INamedTypeSymbol GetTargetType(Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(targetType.FullName);
    }

    protected abstract IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation);

    public string GenerateCode(Compilation compilation, string language)
    {
        var resolvedType = GetTargetType(compilation);
        var methods = GetTargetMethods(compilation).ToList();

        if (language == "CSharp")
        {
            var builder = new CSharpNativeExportBuilder(resolvedType);
            var model = builder.BuildTemplateModel(methods);
            return TemplateManager.RenderTemplate("CSharp.NativeInteropMethod", model);
        }

        LanguageProviderBase languageProvider = language switch
        {
            "Python" => new PythonLanguageProvider(),
            "TypeScript" => new TypeScriptLanguageProvider(),
            "Kotlin" => new KotlinLanguageProvider(),
            _ => throw new NotSupportedException($"Language not supported: {language}")
        };

        var overloads = methods.ComputeOverloads();
        var className = resolvedType.GetGeneratedClassName();

        var customDefinitions = TemplateManager.LoadEmbeddedContent($"{language}.{className}.Object.Defs");
        var customInit = TemplateManager.LoadEmbeddedContent($"{language}.{className}.Object.Init");
        var customClass = TemplateManager.LoadEmbeddedContent($"{language}.{className}.Object.Class");

        var templateModel = languageProvider.BuildClassTemplateModel(
            resolvedType, methods, overloads, InheritFrom,
            customDefinitions, customInit, customClass);

        return TemplateManager.RenderTemplate($"{language}.Object", templateModel);
    }
}
