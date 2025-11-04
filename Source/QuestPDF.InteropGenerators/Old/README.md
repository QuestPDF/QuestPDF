# QuestPDF Interop Generators

This project contains the source generators that automatically create interop bindings for the QuestPDF public API.

## Architecture

The code is organized into 4 separate, well-defined components:

### 1. PublicApiAnalyzer.cs
**Purpose:** Analyzes the QuestPDF public API and extracts methods suitable for interop.

**Responsibilities:**
- Collects all public extension methods from the assembly
- **NEW:** Collects public instance methods from Fluent API classes (descriptors, configurations, handlers, etc.)
- Determines if a method is supported for interop (checks for async, Task-related types, ref/out parameters, generics)
- Validates type compatibility (primitives, structs, enums, classes, interfaces)
- Identifies reference types that need handle boxing
- Filters out async/await patterns and Task-related types
- **NEW:** Identifies Fluent API classes by namespace, naming patterns, and interface implementation

**Key Methods:**
- `CollectExtensionMethods()` - Recursively scans namespaces for public extension methods
- **NEW:** `CollectFluentApiMethods()` - Collects public methods from Fluent API classes
- **NEW:** `CollectAllInteropMethods()` - Collects both extension methods AND Fluent API class methods
- **NEW:** `IsFluentApiClass()` - Determines if a class is part of the Fluent API (checks for Descriptor/Configuration/Handler/Builder suffixes, QuestPDF.Fluent namespace, IContainer interfaces)
- `IsSupported()` - Validates if a method can be exposed via interop
- `IsSupportedType()` - Checks if a type is compatible with UnmanagedCallersOnly
- `IsReferenceType()` - Identifies classes/interfaces that need handle boxing
- `IsTaskRelatedType()` - Detects Task, CancellationToken, and async-related types

**Fluent API Class Detection:**
The analyzer now automatically detects Fluent API classes using multiple strategies:
1. **Namespace-based:** Classes in `QuestPDF.Fluent` namespace
2. **Naming patterns:** Classes ending with `Descriptor`, `Configuration`, `Handler`, `Builder`, or `Settings`
3. **Interface-based:** Classes implementing `IContainer` or `IDocumentContainer`

**Examples of detected Fluent API classes:**
- `TextSpanDescriptor`, `TextPageNumberDescriptor`, `TextBlockDescriptor`, `TextDescriptor`
- `TableColumnsDefinitionDescriptor`, `TableCellDescriptor`, `TableDescriptor`
- `PageDescriptor`, `DecorationDescriptor`, `LayersDescriptor`, `GridDescriptor`
- `ColumnDescriptor`, `RowDescriptor`, `InlinedDescriptor`, `MultiColumnDescriptor`
- `DocumentOperation.LayerConfiguration`, `DocumentOperation.DocumentAttachment`
- And many more...

### 2. CSharpInteropGenerator.cs
**Purpose:** Generates C# UnmanagedCallersOnly bindings for native interop.

**Responsibilities:**
- Creates `GeneratedInterop.g.cs` with all interop methods
- Generates helper methods for handle boxing/unboxing (BoxHandle, UnboxHandle, FreeHandle)
- Converts managed method calls to UnmanagedCallersOnly exports
- **NEW:** Handles both extension methods and instance methods from Fluent API classes
- Handles parameter marshalling (reference types → nint handles)
- **NEW:** For instance methods, automatically adds 'this' parameter as first nint handle
- Generates entry point names (e.g., `questpdf_fluent_alignmentextensions_alignright`)

**Key Methods:**
- `GenerateInteropCode()` - Creates the complete C# interop file
- `GenerateEntryPointName()` - Creates consistent entry point naming
- `GenerateInteropMethod()` - **UPDATED:** Generates wrappers for both extension and instance methods

**Generated Features:**
- UnmanagedCallersOnly methods with CDecl calling convention
- Automatic boxing/unboxing for reference types
- Support for primitives, structs, enums, classes, and interfaces
- Memory management via GCHandle
- **NEW:** Instance method support: automatically unboxes 'this' parameter and calls instance methods
- **NEW:** Distinguishes between static extension methods and instance methods from descriptor classes

**Example for Extension Method:**
```csharp
// Original: public static IContainer AlignCenter(this IContainer container)
[UnmanagedCallersOnly(EntryPoint = "questpdf_fluent_alignmentextensions_aligncenter")]
public static nint AlignmentExtensions_AlignCenter(nint container)
{
    var container_obj = UnboxHandle<IContainer>(container);
    var result = QuestPDF.Fluent.AlignmentExtensions.AlignCenter(container_obj);
    return BoxHandle(result);
}
```

**Example for Instance Method:**
```csharp
// Original: public TextBlockDescriptor AlignCenter() (instance method)
[UnmanagedCallersOnly(EntryPoint = "questpdf_fluent_textblockdescriptor_aligncenter")]
public static nint TextBlockDescriptor_AlignCenter(nint @this)
{
    var this_obj = UnboxHandle<TextBlockDescriptor>(@this);
    var result = this_obj.AlignCenter();
    return BoxHandle(result);
}
```

### 3. PythonBindingsGenerator.cs
**Purpose:** Generates Python ctypes bindings for the interop layer with proper class organization.

**Responsibilities:**
- Creates `GeneratedInterop.g.py.txt` with Python wrapper classes
- **NEW:** Organizes methods into separate Python classes matching C# types (PageDescriptor, ColumnDescriptor, IContainer, etc.)
- Generates ctypes function signatures for all exported methods
- **NEW:** Each C# type gets its own Python class with appropriate methods
- **NEW:** Extension methods and instance methods are unified in their respective Python classes
- Handles both extension methods and instance methods with proper parameter handling

**Key Features:**
- **Class-based architecture:** Instead of all methods on a single `Handle` or `QuestPDFLibrary` class, methods are organized by type
- **Automatic type mapping:** C# types like `IContainer`, `PageDescriptor`, etc. map to Python classes `IContainer`, `PageDescriptor`
- **Fluent API preservation:** Method chaining works naturally in Python as methods return the appropriate typed objects
- **Smart parameter handling:** Extension methods automatically use `self` for the extended type parameter

**Key Methods:**
- `GeneratePythonBindings()` - Orchestrates the complete Python generation process
- `GroupMethodsByType()` - **NEW:** Groups methods by their C# type (extension target or containing type)
- `GenerateLibraryClass()` - Generates the `QuestPDFLibrary` class that manages the native library
- `GeneratePythonWrapperClasses()` - **NEW:** Generates separate Python classes for each C# type
- `GeneratePythonClassMethod()` - **NEW:** Generates methods within their appropriate Python class
- `ToPythonClassName()` - **NEW:** Converts C# type names to Python class names

**Architecture Example:**

```python
# Before (all methods on QuestPDFLibrary or Handle):
lib = QuestPDFLibrary()
container_handle = lib.page_content(document_handle)
aligned_handle = lib.align_center(container_handle)

# After (organized by type):
lib = QuestPDFLibrary()
page = PageDescriptor(lib, page_handle)
container = page.content()  # Returns IContainer
aligned = container.align_center()  # Returns IContainer
```

**Generated Python Classes:**
For each C# type (e.g., `PageDescriptor`, `IContainer`, `ColumnDescriptor`), a Python class is generated with:
- `__init__(self, lib: QuestPDFLibrary, handle: int)` - Constructor accepting library and native handle
- `handle` property - Access to underlying native handle
- Context manager support (`__enter__`, `__exit__`) - For automatic cleanup
- All methods that extend or belong to that type
- Proper return types (returns instances of the appropriate Python class)

**Complete Usage Example:**

```python
from questpdf import QuestPDFLibrary

# Initialize the library
lib = QuestPDFLibrary()

# Create a document
document = lib.create_document()

# Configure pages using PageDescriptor methods
page = document.page()
page.size(PageSize.A4)
page.margin_left(50)
page.margin_right(50)

# Get the content container (returns IContainer)
container = page.content()

# Use IContainer extension methods with fluent chaining
container = container.padding(20)
container = container.align_center()
container = container.background("#FFFFFF")

# Add a column layout (returns ColumnDescriptor)
column = container.column()
column.spacing(10)

# Add items to the column
item1 = column.item()
item1.text("Hello, World!")

item2 = column.item()
item2.text("This is generated from Python!")

# Generate the PDF
document.generate_pdf("output.pdf")
```

**Key Advantages:**

1. **Type Safety:** Each Python class corresponds to a C# type, making the API intuitive
2. **IntelliSense Support:** IDEs can provide better autocomplete based on the typed classes
3. **Method Chaining:** Fluent API patterns work naturally: `container.padding(20).align_center().background("#FFF")`
4. **Clear Ownership:** Methods belong to their logical types rather than being mixed in a single class
5. **Extensibility:** New C# types automatically get their own Python classes
6. **Documentation:** Each class can have type-specific documentation
- Converts C# types to Python types (nint → Handle, int32 → int, etc.)
- Creates Python-friendly method names (PascalCase → snake_case)
- **NEW:** For instance methods, adds 'this_handle' as first parameter
- Implements automatic handle cleanup with context managers

**Key Methods:**
- `GeneratePythonBindings()` - Creates the complete Python bindings file
- `GetPythonCType()` - Maps C# types to ctypes types
- `GetPythonTypeHint()` - Creates Python type hints for IDE support
- `ToPythonMethodName()` - Converts PascalCase to snake_case
- **UPDATED:** `GeneratePythonFunctionSetup()` - Now handles instance methods by adding c_void_p for 'this' parameter
- **UPDATED:** `GeneratePythonWrapperMethod()` - Now generates proper wrappers for instance methods

**Generated Features:**
- `QuestPDFLibrary` class - Main wrapper with automatic library discovery
- `Handle` class - Automatic memory management for managed objects
- Type hints for full IDE support
- Context manager support (with statement)
- Cross-platform library loading (Windows/Linux/macOS)
- **NEW:** Instance method wrappers that accept 'this_handle' as first parameter
- **NEW:** Automatic documentation comments indicating instance vs extension methods

**Example for Extension Method (Python):**
```python
def align_center(self, container: Handle) -> Handle:
    """
    AlignmentExtensions.AlignCenter
    """
    result = self._lib.questpdf_fluent_alignmentextensions_aligncenter(
        container.value if isinstance(container, Handle) else container
    )
    return Handle(self, result)
```

**Example for Instance Method (Python):**
```python
def align_center(self, this_handle: Handle) -> Handle:
    """
    TextBlockDescriptor.AlignCenter
    Instance method - requires handle to the object.
    """
    result = self._lib.questpdf_fluent_textblockdescriptor_aligncenter(
        this_handle.value if isinstance(this_handle, Handle) else this_handle
    )
    return Handle(self, result)
```

### 4. PublicApiGenerator.cs
**Purpose:** Main orchestrator that combines all components.

**Responsibilities:**
- Implements IIncrementalGenerator for Roslyn
- **UPDATED:** Coordinates the 3-step pipeline:
  1. Analyze API → collect **ALL** interop methods (extension methods + Fluent API class methods)
  2. Generate C# code → add to compilation
  3. Generate Python code → **strictly following all C# interop functionalities**
- Registers source outputs with the compiler

**Pipeline Flow:**
```
Compilation
    ↓
PublicApiAnalyzer.CollectAllInteropMethods()
    ├─→ CollectExtensionMethods() (public static extension methods)
    └─→ CollectFluentApiMethods() (public instance methods from descriptors, etc.)
    ↓
[List of IMethodSymbol - BOTH extension methods AND instance methods]
    ↓
    ├─→ CSharpInteropGenerator.GenerateInteropCode()
    │       ↓
    │   GeneratedInterop.g.cs (added to compilation)
    │   - Extension methods: static wrappers
    │   - Instance methods: wrappers with 'this' parameter
    │
    └─→ PythonBindingsGenerator.GeneratePythonBindings()
            ↓
        GeneratedInterop.g.py.txt (available in obj folder)
        - Extension methods: accept container/object handle
        - Instance methods: accept 'this_handle' as first parameter
        - Python bindings strictly mirror C# interop functionality
```

## Complete Fluent API Coverage

The interop generators now provide **complete coverage** of the QuestPDF Fluent API, including:

### 1. Extension Methods (Original Coverage)
All public static extension methods from classes like:
- `AlignmentExtensions`, `PaddingExtensions`, `RowExtensions`, `ColumnExtensions`
- `TextExtensions`, `ImageExtensions`, `TableExtensions`, `PageExtensions`
- `DecorationExtensions`, `LayerExtensions`, `RotateExtensions`, `ScaleExtensions`
- And many more...

### 2. Fluent API Descriptor Classes (NEW)
All public instance methods from descriptor classes including:

**Text Descriptors:**
- `TextSpanDescriptor` - Style(), FontSize(), FontColor(), etc.
- `TextPageNumberDescriptor` - Format(), and all inherited TextSpanDescriptor methods
- `TextBlockDescriptor` - AlignLeft(), AlignCenter(), Justify(), ClampLines(), etc.
- `TextDescriptor` - Span(), Line(), EmptyLine(), CurrentPageNumber(), TotalPages(), etc.

**Table Descriptors:**
- `TableColumnsDefinitionDescriptor` - ConstantColumn(), RelativeColumn()
- `TableCellDescriptor` - Cell()
- `TableDescriptor` - ColumnsDefinition(), Header(), Footer(), Cell(), etc.

**Layout Descriptors:**
- `PageDescriptor` - Size(), Margin(), Content(), Header(), Footer(), etc.
- `ColumnDescriptor` - Item(), Spacing()
- `RowDescriptor` - AutoItem(), RelativeItem(), ConstantItem()
- `DecorationDescriptor` - Before(), Content(), After()
- `LayersDescriptor` - Layer(), PrimaryLayer()
- `GridDescriptor` - Item(), Columns(), Spacing()

**Image/SVG Descriptors:**
- `ImageDescriptor` - FitArea(), FitWidth(), FitHeight(), etc.
- `DynamicImageDescriptor` - Image configuration methods
- `SvgImageDescriptor` - SVG-specific configuration

**Other Descriptors:**
- `InlinedDescriptor` - Item(), Spacing()
- `MultiColumnDescriptor` - Columns(), Spacing()

### 3. Configuration Classes (NEW)
Public instance methods from configuration classes:
- `DocumentOperation.LayerConfiguration` - FilePath, TargetPages, SourcePages, etc.
- `DocumentOperation.DocumentAttachment` - Key, FilePath, AttachmentName, etc.
- And other configuration classes

### 4. Builder/Handler Classes (NEW)
Any public classes with Builder, Handler, or Settings suffix that expose public instance methods.

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

