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
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(context.CompilationProvider, static (spc, compilation) =>
        {
            var mainTemplate = ScribanTemplateLoader.LoadTemplate("Main.cs");
            
            var generators = new List<IInteropSourceGenerator>
            {
                new ContainerSourceGenerator("QuestPDF.Fluent.PaddingExtensions"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.ColumnDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.DecorationDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.InlinedDescriptor"),
            };

            var csharpFragments = generators
                .Select(x => x.GenerateCSharpCode(compilation));
            
            var csharpCode = mainTemplate.Render(new
            {
                GenerationDateTime = DateTime.Now,
                Fragments = csharpFragments
            });

            spc.AddSource("QuestPDF.Interop.g.cs", csharpCode);
        });
    }
}
