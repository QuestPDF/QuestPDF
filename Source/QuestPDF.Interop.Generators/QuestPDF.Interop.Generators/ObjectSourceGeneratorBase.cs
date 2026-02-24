using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using QuestPDF.Interop.Generators.Languages;

namespace QuestPDF.Interop.Generators;

internal abstract class ObjectSourceGeneratorBase : IInteropSourceGenerator
{
    public string? InheritFrom { get; set; }

    protected abstract IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation);
    protected abstract INamedTypeSymbol GetTargetType(Compilation compilation);

    public string GenerateCode(Compilation compilation, string language)
    {
        if (language == "CSharp")
        {
            var builder = new CSharpNativeExportBuilder(GetTargetType(compilation));
            var csharpModel = builder.BuildTemplateModel(GetTargetMethods(compilation));
            return TemplateManager.RenderTemplate("CSharp.NativeInteropMethod", csharpModel);
        }

        ILanguageProvider languageProvider = language switch
        {
            "Python" => new PythonLanguageProvider(),
            "TypeScript" => new TypeScriptLanguageProvider(),
            "Kotlin" => new KotlinLanguageProvider(),
            _ => throw new NotSupportedException($"Language not supported: {language}")
        };

        var targetType = GetTargetType(compilation);
        var methods = GetTargetMethods(compilation).ToList();
        var overloads = methods.ComputeOverloads();
        var className = targetType.GetGeneratedClassName();

        var customDefinitions = TemplateManager.LoadEmbeddedContent($"{language}.{className}.Object.Defs");
        var customInit = TemplateManager.LoadEmbeddedContent($"{language}.{className}.Object.Init");
        var customClass = TemplateManager.LoadEmbeddedContent($"{language}.{className}.Object.Class");

        var templateModel = languageProvider.BuildClassTemplateModel(
            targetType, methods, overloads, InheritFrom,
            customDefinitions, customInit, customClass);

        return TemplateManager.RenderTemplate($"{language}.Object", templateModel);
    }
}
