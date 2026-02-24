using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

internal interface ILanguageProvider
{
    object BuildClassTemplateModel(
        INamedTypeSymbol targetType,
        IReadOnlyList<IMethodSymbol> methods,
        Dictionary<IMethodSymbol, OverloadInfo> overloads,
        string inheritFrom,
        string customDefinitions, string customInit, string customClass);
}

internal enum NameContext
{
    Method,
    Parameter,
    Class,
    EnumValue,
    Property,
    Constant
}
