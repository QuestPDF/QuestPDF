namespace QuestPDF.Interop.Generators.Models;

/// <summary>
/// Language-agnostic model representing type information.
/// </summary>
public class InteropTypeModel
{
    /// <summary>
    /// The original type name in C# (e.g., "System.Int32", "IContainer").
    /// </summary>
    public string OriginalTypeName { get; set; }

    /// <summary>
    /// Short name without namespace (e.g., "Int32", "IContainer").
    /// </summary>
    public string ShortName { get; set; }

    /// <summary>
    /// The kind of type for code generation purposes.
    /// </summary>
    public InteropTypeKind Kind { get; set; }

    /// <summary>
    /// The C-style type representation for FFI (e.g., "int32_t", "void*").
    /// </summary>
    public string NativeType { get; set; }

    /// <summary>
    /// The interop type used in managed wrappers (e.g., "IntPtr", "int").
    /// </summary>
    public string InteropType { get; set; }

    /// <summary>
    /// For Action/Func types, contains type arguments information.
    /// </summary>
    public InteropTypeModel[] TypeArguments { get; set; }

    /// <summary>
    /// For callback types, the C-style typedef name.
    /// </summary>
    public string CallbackTypedefName { get; set; }

    /// <summary>
    /// For callback types, the complete typedef definition.
    /// </summary>
    public string CallbackTypedefDefinition { get; set; }
}

/// <summary>
/// Categorizes types for code generation purposes.
/// </summary>
public enum InteropTypeKind
{
    Void,
    Boolean,
    Integer,
    Float,
    String,
    Enum,
    Class,
    Interface,
    Action,
    Func,
    TypeParameter,
    Color,
    Unknown
}
