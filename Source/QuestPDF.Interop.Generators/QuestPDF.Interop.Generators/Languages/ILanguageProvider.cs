using QuestPDF.Interop.Generators.Models;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Interface for language-specific code generation providers.
/// Each target language implements this interface to customize type mappings,
/// naming conventions, and template model generation.
/// </summary>
public interface ILanguageProvider
{ /// <summary>
    /// Builds a language-specific template model for a class.
    /// </summary>
    object BuildClassTemplateModel(InteropClassModel classModel, string customDefinitions, string customInit, string customClass);
}

/// <summary>
/// Context for name conversion to determine the appropriate naming convention.
/// </summary>
public enum NameContext
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
