using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

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
            var content = NewGenerator.AnalyzeAndGenerate(compilation.Assembly.GlobalNamespace);

            var csharpBuilder = new StringBuilder();
            var pythonBuilder = new StringBuilder();

            var generators = new List<ISourceGenerator>
            {
                new EnumSourceGenerator("QuestPDF.Infrastructure.AspectRatioOption"),
                new EnumSourceGenerator("QuestPDF.Infrastructure.ImageCompressionQuality"),
                new EnumSourceGenerator("QuestPDF.Infrastructure.ImageFormat"),
                new ContainerSourceGenerator() // Generate interop for IContainer extension methods
            };

            foreach (var generator in generators)
            {
                var csharpCodeFragment = generator.GenerateCSharpCode(compilation.Assembly.GlobalNamespace);
                csharpBuilder.AppendLine(csharpCodeFragment);
                
                var pythonCodeFragment = generator.GeneratePythonCode(compilation.Assembly.GlobalNamespace);
                pythonBuilder.AppendLine(pythonCodeFragment);
            }
            
            var csharpCode = csharpBuilder.ToString();
            var pythonCode = pythonBuilder.ToString();

            // Output C# interop code1
            if (!string.IsNullOrWhiteSpace(csharpCode))
            {
                spc.AddSource("QuestPDF.Interop.g.cs", SourceText.From(csharpCode, System.Text.Encoding.UTF8));
            }

            // Output Python bindings code
            if (!string.IsNullOrWhiteSpace(pythonCode))
            {
                //spc.AddSource("QuestPDF.Python.py", SourceText.From(pythonCode, System.Text.Encoding.UTF8));
            }
        });
    }
}
