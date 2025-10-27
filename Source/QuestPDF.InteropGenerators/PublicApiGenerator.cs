using Microsoft.CodeAnalysis;

namespace QuestPDF.InteropGenerators;

/// <summary>
/// Source generator that creates interop bindings for QuestPDF public API.
/// Combines analysis, C# generation, and Python generation into a complete pipeline.
/// </summary>
[Generator]
public sealed class PublicApiGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(context.CompilationProvider, static (spc, compilation) =>
        {
            // Step 1: Analyze the public API and collect all interop methods
            // This includes both extension methods AND public methods from Fluent API classes
            // (descriptors, configurations, handlers, etc.)
            var allMethods = PublicApiAnalyzer.CollectAllInteropMethods(compilation.Assembly.GlobalNamespace);
            
            // Step 2: Generate C# UnmanagedCallersOnly interop code
            var csharpInteropCode = CSharpInteropGenerator.GenerateInteropCode(allMethods);
            spc.AddSource("GeneratedInterop.g.cs", csharpInteropCode);
            
            // Step 3: Generate Python ctypes bindings
            // Python bindings strictly follow all C# interop functionalities
            var pythonBindingsCode = PythonBindingsGenerator.GeneratePythonBindings(allMethods);
            // Note: Python file is added as .txt so it appears in generated files
            // Extract and rename to .py for actual use
            spc.AddSource("GeneratedInterop.g.py.txt", pythonBindingsCode);
        });
    }
}
