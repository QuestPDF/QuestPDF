using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

/// <summary>
/// Interface for generating interop bindings for QuestPDF public API.
/// Generates both C# native AOT code for C ABI/FFI and Python bindings.
/// </summary>
public interface IInteropSourceGenerator
{
    /// <summary>
    /// Generates C# code for native AOT compilation with C ABI compatibility.
    /// This should produce UnmanagedCallersOnly methods for FFI interop.
    /// Return empty string if the type doesn't require C interop code (e.g., enums).
    /// </summary>
    string GenerateCSharpCode(Compilation compilation);

    /// <summary>
    /// Generates Python bindings code using ctypes or similar FFI mechanisms.
    /// This creates Python classes and functions that wrap the C ABI interface.
    /// </summary>
    string GeneratePythonCode(Compilation compilation);
}