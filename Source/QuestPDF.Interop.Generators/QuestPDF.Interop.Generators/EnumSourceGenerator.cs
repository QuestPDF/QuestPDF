using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using QuestPDF.Interop.Generators.Languages;

namespace QuestPDF.Interop.Generators;

public class EnumSourceGenerator : IInteropSourceGenerator
{
    public string GenerateCode(Compilation compilation, string language)
    {
        if (language == "CSharp")
            return string.Empty;

        var model = new TemplateModel
        {
            Enums = GetAllPublicEnumsFromCompilation(compilation).Select(symbol => new TemplateModel.Enum
            {
                Name = language == "Kotlin" ? KotlinLanguageProvider.ConvertTypeName(symbol.Name) : symbol.Name,
                Members = GetEnumMembers(symbol)
            })
        };

        return TemplateManager.RenderTemplate($"{language}.Enum", model);
    }

    class TemplateModel
    {
        public IEnumerable<Enum> Enums { get; set; }

        public class Enum
        {
            public string Name { get; set; }
            public IEnumerable<EnumMember> Members { get; set; }
        }

        public class EnumMember
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }

    private static IEnumerable<INamedTypeSymbol> GetAllPublicEnumsFromCompilation(Compilation compilation)
    {
        return compilation
            .GlobalNamespace
            .GetNamespaceMembers()
            .Where(x => x.Name.StartsWith("QuestPDF"))
            .SelectMany(x => x.GetMembersRecursively())
            .Where(x => x.TypeKind == TypeKind.Enum)
            .Where(x => x.DeclaredAccessibility == Accessibility.Public);
    }

    private static IEnumerable<TemplateModel.EnumMember> GetEnumMembers(INamedTypeSymbol symbol)
    {
        return symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(x => x.HasConstantValue)
            .Select(x => new TemplateModel.EnumMember
            {
                Name = x.Name,
                Value = System.Convert.ToInt32(x.ConstantValue)
            });
    }
}
