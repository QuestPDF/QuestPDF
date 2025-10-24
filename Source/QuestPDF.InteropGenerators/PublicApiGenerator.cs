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
            // Step 1: Analyze the public API and collect extension methods
            var extensionMethods = PublicApiAnalyzer.CollectExtensionMethods(compilation.Assembly.GlobalNamespace);
            
            // Step 2: Generate C# UnmanagedCallersOnly interop code
            var csharpInteropCode = CSharpInteropGenerator.GenerateInteropCode(extensionMethods);
            spc.AddSource("GeneratedInterop.g.cs", csharpInteropCode);
            
            // Step 3: Generate Python ctypes bindings
            var pythonBindingsCode = PythonBindingsGenerator.GeneratePythonBindings(extensionMethods);
            // Note: Python file is added as .txt so it appears in generated files
            // Extract and rename to .py for actual use
            spc.AddSource("GeneratedInterop.g.py.txt", pythonBindingsCode);
        });
    }
}
