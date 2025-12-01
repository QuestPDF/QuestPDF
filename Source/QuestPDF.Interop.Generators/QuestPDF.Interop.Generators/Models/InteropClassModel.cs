using System.Collections.Generic;

namespace QuestPDF.Interop.Generators.Models;

/// <summary>
/// Language-agnostic model representing a class/descriptor to be generated.
/// </summary>
public class InteropClassModel
{
    /// <summary>
    /// The original type name in C# (e.g., "ColumnDescriptor", "IContainer").
    /// </summary>
    public string OriginalTypeName { get; set; }

    /// <summary>
    /// The name to use in generated code (e.g., "ColumnDescriptor", "Container").
    /// Interfaces have the 'I' prefix removed.
    /// </summary>
    public string GeneratedClassName { get; set; }

    public bool IsStaticClass { get; set; }
    
    /// <summary>
    /// Whether the original type is an interface.
    /// </summary>
    public bool IsInterface { get; set; }

    /// <summary>
    /// All methods to be exposed for this class.
    /// </summary>
    public IReadOnlyList<InteropMethodModel> Methods { get; set; }

    /// <summary>
    /// All unique callback typedef definitions needed by methods in this class.
    /// </summary>
    public IReadOnlyList<string> CallbackTypedefs { get; set; }

    /// <summary>
    /// All C header function signatures for FFI declarations.
    /// </summary>
    public IReadOnlyList<string> CHeaderSignatures { get; set; }
}
