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

        var model = new
        {
            Enums = compilation
                .GetAllQuestPdfTypes()
                .Where(x => x.TypeKind == TypeKind.Enum && x.DeclaredAccessibility == Accessibility.Public)
                .Select(symbol => new
                {
                    Name = language == "Kotlin" ? KotlinLanguageProvider.ConvertTypeName(symbol.Name) : symbol.Name,
                    Members = GetEnumMembers(symbol)
                })
        };

        return TemplateManager.RenderTemplate($"{language}.Enum", model);
    }

    private static IEnumerable<object> GetEnumMembers(INamedTypeSymbol symbol)
    {
        return symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(x => x.HasConstantValue)
            .Select(x => new
            {
                x.Name, 
                Value = System.Convert.ToInt32(x.ConstantValue)
            });
    }
}
