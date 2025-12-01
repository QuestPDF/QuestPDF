using System.Collections.Generic;

namespace QuestPDF.Interop.Generators.Models;

/// <summary>
/// Language-agnostic model representing a method to be exposed via interop.
/// This model captures all the metadata needed to generate code for any target language.
/// </summary>
public class InteropMethodModel
{
    /// <summary>
    /// The original method name as defined in C# (e.g., "SetMargin").
    /// </summary>
    public string OriginalName { get; set; }

    /// <summary>
    /// Unique native function name used for FFI (e.g., "questpdf__container__set_margin__a1b2c3d4").
    /// </summary>
    public string NativeEntryPoint { get; set; }

    /// <summary>
    /// Name used for the managed (C#) method wrapper.
    /// </summary>
    public string ManagedMethodName { get; set; }

    public bool IsStaticMethod { get; set; }
    
    /// <summary>
    /// Whether this method is an extension method.
    /// </summary>
    public bool IsExtensionMethod { get; set; }

    /// <summary>
    /// The deprecation message if the method is marked obsolete, null otherwise.
    /// </summary>
    public string DeprecationMessage { get; set; }

    /// <summary>
    /// Information about the return type.
    /// </summary>
    public InteropTypeModel ReturnType { get; set; }

    /// <summary>
    /// Parameters of the method (excluding 'this' parameter for extension methods).
    /// </summary>
    public IReadOnlyList<InteropParameterModel> Parameters { get; set; }

    /// <summary>
    /// Callback parameters that require special handling.
    /// </summary>
    public IReadOnlyList<InteropCallbackModel> Callbacks { get; set; }

    /// <summary>
    /// C-style function signature header for FFI declarations.
    /// </summary>
    public string CHeaderSignature { get; set; }

    // =========================================================================
    // Overload handling properties (set during preprocessing)
    // =========================================================================

    /// <summary>
    /// Whether this method is part of an overload group (multiple methods with the same name).
    /// </summary>
    public bool IsOverload { get; set; }

    /// <summary>
    /// For overloaded methods, this contains the unique disambiguated name
    /// (e.g., "SetMargin_Float" or "SetMargin_Float_Float").
    /// For non-overloaded methods, this equals OriginalName.
    /// </summary>
    public string DisambiguatedName { get; set; }

    /// <summary>
    /// A suffix describing the parameter types, used for disambiguation
    /// (e.g., "_Float_String" or "_Action").
    /// Empty for non-overloaded methods.
    /// </summary>
    public string OverloadSuffix { get; set; }
}
