# QuestPDF Interop Generators

This project contains the source generators that automatically create interop bindings for the QuestPDF public API.

## Architecture

The code is organized into 4 separate, well-defined components:

### 1. PublicApiAnalyzer.cs
**Purpose:** Analyzes the QuestPDF public API and extracts methods suitable for interop.

**Responsibilities:**
- Collects all public extension methods from the assembly
- Determines if a method is supported for interop (checks for async, Task-related types, ref/out parameters, generics)
- Validates type compatibility (primitives, structs, enums, classes, interfaces)
- Identifies reference types that need handle boxing
- Filters out async/await patterns and Task-related types

**Key Methods:**
- `CollectExtensionMethods()` - Recursively scans namespaces for public extension methods
- `IsSupported()` - Validates if a method can be exposed via interop
- `IsSupportedType()` - Checks if a type is compatible with UnmanagedCallersOnly
- `IsReferenceType()` - Identifies classes/interfaces that need handle boxing
- `IsTaskRelatedType()` - Detects Task, CancellationToken, and async-related types

### 2. CSharpInteropGenerator.cs
**Purpose:** Generates C# UnmanagedCallersOnly bindings for native interop.

**Responsibilities:**
- Creates `GeneratedInterop.g.cs` with all interop methods
- Generates helper methods for handle boxing/unboxing (BoxHandle, UnboxHandle, FreeHandle)
- Converts managed method calls to UnmanagedCallersOnly exports
- Handles parameter marshalling (reference types → nint handles)
- Generates entry point names (e.g., `questpdf_fluent_alignmentextensions_alignright`)

**Key Methods:**
- `GenerateInteropCode()` - Creates the complete C# interop file
- `GenerateEntryPointName()` - Creates consistent entry point naming
- `GenerateInteropMethod()` - Generates individual method wrappers

**Generated Features:**
- UnmanagedCallersOnly methods with CDecl calling convention
- Automatic boxing/unboxing for reference types
- Support for primitives, structs, enums, classes, and interfaces
- Memory management via GCHandle

### 3. PythonBindingsGenerator.cs
**Purpose:** Generates Python ctypes bindings for the interop layer.

**Responsibilities:**
- Creates `GeneratedInterop.g.py.txt` with Python wrapper classes
- Generates ctypes function signatures for all exported methods
- Converts C# types to Python types (nint → Handle, int32 → int, etc.)
- Creates Python-friendly method names (PascalCase → snake_case)
- Implements automatic handle cleanup with context managers

**Key Methods:**
- `GeneratePythonBindings()` - Creates the complete Python bindings file
- `GetPythonCType()` - Maps C# types to ctypes types
- `GetPythonTypeHint()` - Creates Python type hints for IDE support
- `ToPythonMethodName()` - Converts PascalCase to snake_case

**Generated Features:**
- `QuestPDFLibrary` class - Main wrapper with automatic library discovery
- `Handle` class - Automatic memory management for managed objects
- Type hints for full IDE support
- Context manager support (with statement)
- Cross-platform library loading (Windows/Linux/macOS)

### 4. PublicApiGenerator.cs
**Purpose:** Main orchestrator that combines all components.

**Responsibilities:**
- Implements IIncrementalGenerator for Roslyn
- Coordinates the 3-step pipeline:
  1. Analyze API → collect methods
  2. Generate C# code → add to compilation
  3. Generate Python code → add as text file
- Registers source outputs with the compiler

**Pipeline Flow:**
```
Compilation
    ↓
PublicApiAnalyzer.CollectExtensionMethods()
    ↓
[List of IMethodSymbol]
    ↓
    ├─→ CSharpInteropGenerator.GenerateInteropCode()
    │       ↓
    │   GeneratedInterop.g.cs (added to compilation)
    │
    └─→ PythonBindingsGenerator.GeneratePythonBindings()
            ↓
        GeneratedInterop.g.py.txt (available in obj folder)
```

## Type Support

### Supported Types
- **Primitives:** bool, byte, sbyte, short, ushort, int, uint, long, ulong, float, double
- **Special:** IntPtr, UIntPtr
- **Value Types:** Structs (must be blittable), Enums
- **Reference Types:** Classes and Interfaces (passed as handles)
- **Pointers:** Raw pointers and function pointers

### Unsupported Types
- **Async:** async methods, Task, Task<T>, ValueTask, CancellationToken
- **Complex:** Generic methods, Arrays, Delegates
- **Modifiers:** ref, out, in parameters

## Usage

### Building
The generators run automatically during compilation:
```bash
dotnet build QuestPDF/QuestPDF.csproj
```

### Output Files
- **C# Interop:** `QuestPDF/obj/Debug/netX.0/generated/.../GeneratedInterop.g.cs`
- **Python Bindings:** `QuestPDF/obj/Debug/netX.0/generated/.../GeneratedInterop.g.py.txt`

### Python Example
```python
from questpdf import QuestPDFLibrary, Handle

# Initialize library
pdf = QuestPDFLibrary()

# Use the API (methods converted to snake_case)
container = pdf.create_container()
with container as c:
    aligned = pdf.align_right(c)  # Returns Handle
    padded = pdf.padding(aligned, 10)
    # Handles automatically freed when out of scope
```

## Memory Management

### C# Side
- Uses GCHandle to pin managed objects
- Objects kept alive until explicitly freed
- `questpdf_free_handle` exported for cleanup

### Python Side
- `Handle` class wraps nint pointers
- Automatic cleanup via `__del__` and context managers
- Prevents double-free with handle tracking

## Extension Points

To add support for new types:
1. Update `PublicApiAnalyzer.IsSupportedType()`
2. Update `CSharpInteropGenerator` for C# marshalling
3. Update `PythonBindingsGenerator.GetPythonCType()` for Python mapping

To exclude specific methods:
1. Add filtering logic to `PublicApiAnalyzer.IsSupported()`

