namespace QuestPDF.Interop.Generators.Models;

/// <summary>
/// Language-agnostic model representing a method parameter.
/// </summary>
public class InteropParameterModel
{
    /// <summary>
    /// The original parameter name in C# (e.g., "marginValue").
    /// </summary>
    public string OriginalName { get; set; }

    /// <summary>
    /// Information about the parameter's type.
    /// </summary>
    public InteropTypeModel Type { get; set; }

    /// <summary>
    /// Whether this parameter has a default value.
    /// </summary>
    public bool HasDefaultValue { get; set; }

    /// <summary>
    /// The default value if present. Type depends on the parameter type:
    /// - null for reference types
    /// - Primitive values (int, bool, string, etc.)
    /// - Enum constant value (as int)
    /// </summary>
    public object DefaultValue { get; set; }

    /// <summary>
    /// For enum types with default values, contains the enum member name.
    /// </summary>
    public string DefaultEnumMemberName { get; set; }
}
