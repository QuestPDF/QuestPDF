using System;
using System.Collections.Generic;
using System.Globalization;
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
                new EnumSourceGenerator(),
                
                new ContainerSourceGenerator([
                    "QuestPDF.Fluent.PaddingExtensions",
                    "QuestPDF.Fluent.ExtendExtensions",
                    "QuestPDF.Fluent.ShrinkExtensions",
                    "QuestPDF.Fluent.TranslateExtensions"
                ]),
                // new DescriptorSourceGenerator("QuestPDF.Fluent.ColumnDescriptor"),
                // new DescriptorSourceGenerator("QuestPDF.Fluent.InlinedDescriptor"),
            };
            
            GenerateCode("QuestPDF.Interop.g.cs", "Main.cs", x => x.GenerateCSharpCode(compilation));
            GenerateCode("QuestPDF.Interop.g.py", "Main.py", x => x.GeneratePythonCode(compilation));
            
            void GenerateCode(string sourceFileName, string templateName, Func<IInteropSourceGenerator, string> selector)
            {
                var codeFragments = generators
                    .Select(x => Try(() => x.GenerateCSharpCode(compilation)));
            
                var csharpCode = ScribanTemplateLoader
                    .LoadTemplate(templateName)
                    .Render(new
                    {
                        GenerationDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                        Fragments = codeFragments
                    });

                spc.AddSource(sourceFileName, csharpCode);
            }
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
