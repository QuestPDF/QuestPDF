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
            // new EnumSourceGenerator(),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.ColumnDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.DecorationDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.InlinedDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.LayersDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.RowDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.GridDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.MultiColumnDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.TableDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.TableCellDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.TableColumnsDefinitionDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.TextDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.TextSpanDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.TextPageNumberDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.TextBlockDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.ImageDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.DynamicImageDescriptor"),
            // new DescriptorSourceGenerator("QuestPDF.Fluent.SvgImageDescriptor"),
            // new ContainerSourceGenerator()
        };
        
        GenerateCode("QuestPDF.Interop.g.cs", "CSharp.Main", x => x.GenerateCSharpCode(compilation));
        GenerateCode("QuestPDF.Interop.g.py", "Python.Main", x => x.GeneratePythonCode(compilation));
        GenerateCode("QuestPDF.Interop.g.java", "Java.Main", x => x.GenerateJavaCode(compilation));
        GenerateCode("QuestPDF.Interop.g.ts", "TypeScript.Main", x => x.GenerateTypeScriptCode(compilation));
        
        void GenerateCode(string sourceFileName, string templateName, Func<IInteropSourceGenerator, string> selector)
        {
            var codeFragments = generators
                .Select(x => Try(() => selector(x)));
                
            var finalCode = ScribanTemplateLoader
                .LoadTemplate(templateName)
                .Render(new
                {
                    GenerationDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    Fragments = codeFragments
                });
            
            var path = Path.Combine("Generated", sourceFileName);
            
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
