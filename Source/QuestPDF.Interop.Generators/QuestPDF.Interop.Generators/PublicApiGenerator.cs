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
            var mainTemplate = ScribanTemplateLoader.LoadTemplate("Main.cs");
            
            var generators = new List<IInteropSourceGenerator>
            {
                new ContainerSourceGenerator("QuestPDF.Fluent.PaddingExtensions"),
                //new ContainerSourceGenerator("QuestPDF.Fluent.ColumnExtensions"),
                //new ContainerSourceGenerator("QuestPDF.Fluent.InlinedExtensions"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.ColumnDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.InlinedDescriptor"),
            };

            var csharpFragments = generators
                .Select(x => x.GenerateCSharpCode(compilation));
            
            var csharpCode = mainTemplate.Render(new
            {
                GenerationDateTime = DateTime.Now.ToString(),
                Fragments = csharpFragments
            });

            spc.AddSource("QuestPDF.Interop.g.cs", csharpCode);
        });
    }
}
