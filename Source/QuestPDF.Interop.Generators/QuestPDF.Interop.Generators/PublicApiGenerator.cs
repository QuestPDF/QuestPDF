using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace QuestPDF.Interop.Generators;

[Generator]
public sealed class PublicApiGenerator : IIncrementalGenerator
{
    public PublicApiGenerator()
    {
        //LoadDependencies();
    }
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(context.CompilationProvider, static (spc, compilation) =>
        {
            var csharpBuilder = new StringBuilder();
            var pythonBuilder = new StringBuilder();
            
            csharpBuilder.AppendLine($"// AUTO-GENERATED on {DateTime.Now}\n");
            
            var generators = new List<IInteropSourceGenerator>
            {
                new ContainerSourceGenerator("QuestPDF.Fluent.PaddingExtensions"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.ColumnDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.DecorationDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.InlinedDescriptor"),
            };
            
            var sw = Stopwatch.StartNew();
            
            foreach (var generator in generators)
            {
                try
                {
                    var csharpCodeFragment = generator.GenerateCSharpCode(compilation);
                    csharpBuilder.AppendLine(csharpCodeFragment);
                }
                catch (Exception e)
                {
                    csharpBuilder.AppendLine($"Error: {e}");
                }
            }

            csharpBuilder.AppendLine($"// Generation completed in {sw.ElapsedMilliseconds} ms");
            
            // Output C# interop code1
            spc.AddSource("QuestPDF.Interop.g.cs", csharpBuilder.ToString());

            
        });
    }
}
