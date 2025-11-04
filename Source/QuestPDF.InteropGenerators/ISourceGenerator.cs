using Microsoft.CodeAnalysis;

namespace QuestPDF.InteropGenerators;

/// <summary>
/// Interface for generating interop bindings for QuestPDF public API.
/// Generates both C# native AOT code for C ABI/FFI and Python bindings.
/// </summary>
public interface ISourceGenerator
{
    /// <summary>
    /// Generates C# code for native AOT compilation with C ABI compatibility.
    /// This should produce UnmanagedCallersOnly methods for FFI interop.
    /// Return empty string if the type doesn't require C interop code (e.g., enums).
    /// </summary>
    /// <param name="namespaceSymbol">Root namespace symbol to analyze</param>
    /// <returns>Generated C# interop code or empty string if not applicable</returns>
    string GenerateCSharpCode(INamespaceSymbol namespaceSymbol);

    /// <summary>
    /// Generates Python bindings code using ctypes or similar FFI mechanisms.
    /// This creates Python classes and functions that wrap the C ABI interface.
    /// </summary>
    /// <param name="namespaceSymbol">Root namespace symbol to analyze</param>
    /// <returns>Generated Python binding code</returns>
    string GeneratePythonCode(INamespaceSymbol namespaceSymbol);
}