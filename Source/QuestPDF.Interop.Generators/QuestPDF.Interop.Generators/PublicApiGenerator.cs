using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

[Generator]
public sealed class PublicApiGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(context.CompilationProvider, static (spc, compilation) =>
        {
            var generators = new List<IInteropSourceGenerator>
            {
                new EnumSourceGenerator("QuestPDF.Infrastructure.Unit"),
                
                new ContainerSourceGenerator("QuestPDF.Fluent.PaddingExtensions"),
                //new ContainerSourceGenerator("QuestPDF.Fluent.ColumnExtensions"),
                //new ContainerSourceGenerator("QuestPDF.Fluent.InlinedExtensions"),
                // new DescriptorSourceGenerator("QuestPDF.Fluent.ColumnDescriptor"),
                // new DescriptorSourceGenerator("QuestPDF.Fluent.InlinedDescriptor"),
            };

            // C# Generation
            var csharpFragments = generators
                .Select(x => Try(() => x.GenerateCSharpCode(compilation)));
            
            var csharpCode = ScribanTemplateLoader.LoadTemplate("Main.cs").Render(new
            {
                GenerationDateTime = DateTime.Now.ToString(),
                Fragments = csharpFragments
            });

            spc.AddSource("QuestPDF.Interop.g.cs", csharpCode);
            
            // Python Generation
            var pythonFragments = generators
                .Select(x => Try(() => x.GeneratePythonCode(compilation)));
            
            var pythonCode = ScribanTemplateLoader.LoadTemplate("Main.py").Render(new
            {
                GenerationDateTime = DateTime.Now.ToString(),
                Fragments = pythonFragments
            });
            
            spc.AddSource("QuestPDF.Interop.g.py", pythonCode);
        });
    }
    
    private static string Try(Func<string> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            return $"// Generation error: {ex.Message}";
        }
    }
}
