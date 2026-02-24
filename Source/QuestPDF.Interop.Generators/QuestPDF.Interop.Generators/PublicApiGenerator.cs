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
        var listOfAvailableMethods = compilation
            .GlobalNamespace
            .GetNamespaceMembers()
            .Where(x => x.Name.StartsWith("QuestPDF"))
            .SelectMany(x => x.GetMembersRecursively())
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .SelectMany(x => x.GetMembers())
            .OfType<IMethodSymbol>()
            .ExcludeOldObsoleteMethods()
            .Where(x => x.MethodKind is not (MethodKind.PropertyGet or MethodKind.PropertySet))
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .Select(x => x.ToDisplayString())
            .ToList();
        
        var generators = new List<IInteropSourceGenerator>
        {
            new ColorsSourceGenerator(),
            new EnumSourceGenerator(),
            new PlainSourceLoader("Settings"),
            new PlainSourceLoader("PageSizes"),
            new SimpleSourceGenerator(typeof(QuestPDF.Helpers.FontFeatures)),
            new SimpleSourceGenerator(typeof(QuestPDF.Helpers.Placeholders)),
            new SimpleSourceGenerator(typeof(QuestPDF.Drawing.FontManager))
            {
                ExcludeMembers = [ "RegisterFontFromEmbeddedResource" ]
            },
            new SimpleSourceGenerator(typeof(QuestPDF.Infrastructure.Image)),
            new SimpleSourceGenerator(typeof(QuestPDF.Infrastructure.SvgImage)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.LineDescriptor))
            {
                ExcludeMembers = [ "LineDashPattern", "LineGradient" ]
            },
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.ColumnDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.DecorationDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.InlinedDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.LayersDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.RowDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.GridDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.MultiColumnDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.TableCellDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.TableColumnsDefinitionDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.TableDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.TextDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.TextSpanDescriptor))
            {
                IncludeInheritedExtensionMethods = true
            },
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.TextPageNumberDescriptor))
            {
                IncludeInheritedExtensionMethods = true
            },
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.TextBlockDescriptor))
            {
                IncludeInheritedExtensionMethods = true
            },
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.ImageDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.DynamicImageDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.SvgImageDescriptor)),
            new DescriptorSourceGenerator(typeof(QuestPDF.Fluent.PageDescriptor)),
            new ExtensionMethodSourceGenerator(typeof(QuestPDF.Infrastructure.IContainer))
            {
                ExcludeMembers = [
                    "BackgroundLinearGradient",
                    "BorderLinearGradient",
                    "QuestPDF.Fluent.ImageExtensions.Image(QuestPDF.Infrastructure.IContainer, System.Func<QuestPDF.Infrastructure.ImageSize, byte[]>)",
                    "QuestPDF.Fluent.SvgExtensions.Svg(QuestPDF.Infrastructure.IContainer, System.Func<QuestPDF.Infrastructure.Size, string>)"
                ]
            },
            new ExtensionMethodSourceGenerator(typeof(QuestPDF.Elements.Table.ITableCellContainer))
            {
                InheritFrom = "Container"
            },
        };
        
        var languages = new (string FileName, string Language)[]
        {
            ("QuestPDF.Interop.g.cs", "CSharp"),
            ("QuestPDF.Interop.g.py", "Python"),
            ("QuestPDF.Interop.g.ts", "TypeScript"),
            ("QuestPDF.Interop.g.kt", "Kotlin"),
        };

        Directory.CreateDirectory("generated");

        foreach (var (fileName, language) in languages)
        {
            var fragments = generators
                .Select(g => g.GenerateCode(compilation, language))
                .ToList();

            var finalCode = TemplateManager.RenderTemplate($"{language}.Main", new
            {
                GenerationDateTime = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                Fragments = fragments
            });

            File.WriteAllText(Path.Combine("generated", fileName), finalCode);
        }
    }
}
