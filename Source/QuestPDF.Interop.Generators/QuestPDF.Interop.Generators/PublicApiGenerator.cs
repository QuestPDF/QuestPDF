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
                new DescriptorSourceGenerator("QuestPDF.Fluent.ColumnDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.DecorationDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.InlinedDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.LayersDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.RowDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.GridDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.MultiColumnDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.TableDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.TableCellDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.TableColumnsDefinitionDescriptor"),
                new DescriptorSourceGenerator("QuestPDF.Fluent.TextDescriptor"),
                new ContainerSourceGenerator()
            };
            
            GenerateCode("QuestPDF.Interop.g.cs", "Main.cs", x => x.GenerateCSharpCode(compilation));
            GenerateCode("QuestPDF.Interop.g.py", "Main.py", x => x.GeneratePythonCode(compilation));
            
            void GenerateCode(string sourceFileName, string templateName, Func<IInteropSourceGenerator, string> selector)
            {
                var codeFragments = generators
                    .Select(x => Try(() => selector(x)));
            
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
            return $"// Generation error:\n\n{ex.Message}\n\n{ex.StackTrace}";
        }
    }
}
