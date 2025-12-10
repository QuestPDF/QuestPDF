using QuestPDF.Interop.Generators.Models;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Interface for language-specific code generation providers.
/// Each target language implements this interface to customize type mappings,
/// naming conventions, and template model generation.
/// </summary>
public interface ILanguageProvider
{
    /// <summary>
    /// The name of the target language (e.g., "Python", "Java", "TypeScript").
    /// </summary>
    string LanguageName { get; }

    /// <summary>
    /// The file extension for generated files (e.g., ".py", ".java", ".ts").
    /// </summary>
    string FileExtension { get; }

    /// <summary>
    /// Converts a C# name to the target language's naming convention.
    /// </summary>
    string ConvertName(string csharpName, NameContext context);

    /// <summary>
    /// Gets the target language type for a given interop type model.
    /// </summary>
    string GetTargetType(InteropTypeModel type);

    /// <summary>
    /// Formats a default value for the target language.
    /// </summary>
    string FormatDefaultValue(InteropParameterModel parameter);

    /// <summary>
    /// Gets the value representation for passing to interop calls.
    /// </summary>
    string GetInteropValue(InteropParameterModel parameter, string variableName);

    /// <summary>
    /// Gets the template name for the main output file.
    /// </summary>
    string MainTemplateName { get; }

    /// <summary>
    /// Gets the template name for object/class generation.
    /// </summary>
    string ObjectTemplateName { get; }

    /// <summary>
    /// Gets the template name for enum generation.
    /// </summary>
    string EnumTemplateName { get; }

    /// <summary>
    /// Gets the template name for colors generation.
    /// </summary>
    string ColorsTemplateName { get; }

    /// <summary>
    /// Builds a language-specific template model for a class.
    /// </summary>
    object BuildClassTemplateModel(InteropClassModel classModel, string customDefinitions, string customInit, string customClass);

    /// <summary>
    /// Builds a language-specific template model for enums.
    /// </summary>
    object BuildEnumTemplateModel(object enums);

    /// <summary>
    /// Builds a language-specific template model for colors.
    /// </summary>
    object BuildColorsTemplateModel(object basicColors, object colorGroups);
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
