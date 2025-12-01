using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

public sealed class PublicApiGenerator
{
    public static void GenerateSource(Compilation compilation)
    {
        var generators = new List<IInteropSourceGenerator>
        {
            new ColorsSourceGenerator(),
            new EnumSourceGenerator(),
            new SimpleSourceGenerator("QuestPDF.Helpers.FontFeatures"),
            new SimpleSourceGenerator("QuestPDF.Helpers.Placeholders"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.LineDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.ColumnDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.DecorationDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.InlinedDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.LayersDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.RowDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.GridDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.MultiColumnDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.TableCellDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.TableColumnsDefinitionDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.TableDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.TextDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.TextSpanDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.TextPageNumberDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.TextBlockDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.ImageDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.DynamicImageDescriptor"),
            new DescriptorSourceGenerator("QuestPDF.Fluent.SvgImageDescriptor"),
            new ContainerSourceGenerator()
            {
                ExtensionTemplateName = "Container"
            }
        };
        
        GenerateCode("QuestPDF.Interop.g.cs", "CSharp.Main", x => x.GenerateCSharpCode(compilation));
        GenerateCode("QuestPDF.Interop.g.py", "Python.Main", x => x.GeneratePythonCode(compilation));
        GenerateCode("QuestPDF.Interop.g.ts", "TypeScript.Main", x => x.GenerateTypeScriptCode(compilation));
        GenerateCode("QuestPDF.Interop.g.kt", "Kotlin.Main", x => x.GenerateKotlinCode(compilation));
        
        void GenerateCode(string sourceFileName, string templateName, Func<IInteropSourceGenerator, string> selector)
        {
            var codeFragments = generators
                .Select(x => Try(() => selector(x)));
                
            var finalCode = TemplateManager
                .RenderTemplate(templateName, new
                {
                    GenerationDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    Fragments = codeFragments.ToList()
                });
            
            Directory.CreateDirectory("generated");
            
            var path = Path.Combine("generated", sourceFileName);
            
            if (File.Exists(path))
                File.Delete(path);
            
            File.WriteAllText(path, finalCode);
        }
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
