namespace QuestPDF.Interop.Generators.Models;

/// <summary>
/// Language-agnostic model representing callback/delegate parameters.
/// </summary>
public class InteropCallbackModel
{
    /// <summary>
    /// The original parameter name in C# (e.g., "configurator").
    /// </summary>
    public string ParameterName { get; set; }

    /// <summary>
    /// The type name of the callback argument (e.g., "IContainer" becomes "Container").
    /// </summary>
    public string ArgumentTypeName { get; set; }

    /// <summary>
    /// Information about the callback argument type.
    /// </summary>
    public InteropTypeModel ArgumentType { get; set; }

    /// <summary>
    /// For Func callbacks, information about the return type.
    /// </summary>
    public InteropTypeModel ReturnType { get; set; }

    /// <summary>
    /// Whether this is a Func (has return value) vs Action (void return).
    /// </summary>
    public bool HasReturnValue { get; set; }

    /// <summary>
    /// C-style callback signature for FFI (e.g., "void(void*)").
    /// </summary>
    public string NativeSignature { get; set; }
}
