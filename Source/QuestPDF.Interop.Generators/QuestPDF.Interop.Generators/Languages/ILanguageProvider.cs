using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Interface for language-specific code generation providers.
/// Each target language implements this interface to customize type mappings,
/// naming conventions, and template model generation.
/// </summary>
internal interface ILanguageProvider
{
    /// <summary>
    /// Builds a language-specific template model for a class directly from Roslyn symbols.
    /// </summary>
    object BuildClassTemplateModel(
        INamedTypeSymbol targetType,
        IReadOnlyList<IMethodSymbol> methods,
        Dictionary<IMethodSymbol, OverloadInfo> overloads,
        string inheritFrom,
        string customDefinitions, string customInit, string customClass);
}

/// <summary>
/// Context for name conversion to determine the appropriate naming convention.
/// </summary>
internal enum NameContext
{
    /// <summary>Method name (e.g., snake_case in Python, camelCase in Java)</summary>
    Method,
    /// <summary>Parameter name</summary>
    Parameter,
    /// <summary>Class name (typically PascalCase)</summary>
    Class,
    /// <summary>Enum value name</summary>
    EnumValue,
    /// <summary>Property name</summary>
    Property,
    /// <summary>Constant name</summary>
    Constant
}
