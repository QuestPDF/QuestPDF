using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace QuestPDF.Interop.Generators;

internal class ColorsSourceGenerator : IInteropSourceGenerator
{
    public string GenerateCode(Compilation compilation, string language)
    {
        if (language == "CSharp")
            return string.Empty;

        var colorType = compilation.GetTypeByMetadataName("QuestPDF.Infrastructure.Color");
        var colorsClass = compilation.GetTypeByMetadataName("QuestPDF.Helpers.Colors")!;

        var model = new
        {
            BasicColors = GetColorFields(colorsClass, colorType),
            ColorGroups = colorsClass.GetTypeMembers()
                .Where(t => t.TypeKind == TypeKind.Class)
                .Select(t => new { t.Name, Colors = GetColorFields(t, colorType) })
        };

        return TemplateManager.RenderTemplate($"{language}.Colors", model);
    }

    private static IEnumerable<object> GetColorFields(INamedTypeSymbol type, INamedTypeSymbol colorType)
    {
        return type.GetMembers()
            .Where(x => x.IsStatic)
            .OfType<IFieldSymbol>()
            .Where(x => SymbolEqualityComparer.Default.Equals(x.Type, colorType))
            .Select(x => new { x.Name, Value = GetColorValue(x) });
    }

    private static string GetColorValue(IFieldSymbol field)
    {
        var declarator = (VariableDeclaratorSyntax)field.DeclaringSyntaxReferences.Single().GetSyntax();
        var creation = (BaseObjectCreationExpressionSyntax)declarator.Initializer!.Value;
        return creation.ArgumentList!.Arguments.Single().Expression.ToString();
    }
}
