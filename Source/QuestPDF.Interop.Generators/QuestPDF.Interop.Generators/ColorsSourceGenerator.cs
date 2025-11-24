using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace QuestPDF.Interop.Generators;

public class ColorsSourceGenerator : IInteropSourceGenerator
{
    public string GenerateCSharpCode(Compilation compilation)
    {
        return string.Empty;
    }

    public string GeneratePythonCode(Compilation compilation)
    {
        var colorType = compilation.GetTypeByMetadataName("QuestPDF.Infrastructure.Color");
        var colorClass = compilation.GetTypeByMetadataName("QuestPDF.Helpers.Colors");

        var model = new
        {
            BasicColors = GetBasicColorsOf(colorClass),
            ColorGroups = colorClass
                .GetTypeMembers()
                .Where(t => t.TypeKind == TypeKind.Class)
                .Select(t => new
                {
                    Name = t.Name.ToSnakeCase(),
                    Colors = GetBasicColorsOf(t)
                })
        };
        
        return ScribanTemplateLoader.LoadTemplate("Colors.py").Render(model);

        IEnumerable<object> GetBasicColorsOf(INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol
                .GetMembers()
                .Where(x => x.IsStatic && x is IFieldSymbol fieldSymbol && SymbolEqualityComparer.Default.Equals(fieldSymbol.Type, colorType))
                .Select(x => new
                {
                    Name = x.Name.ToSnakeCase(),
                    Value = GetColorValue(x as IFieldSymbol)
                });
        }

        static string GetColorValue(IFieldSymbol fieldSymbol)
        {
            var syntaxRef = fieldSymbol.DeclaringSyntaxReferences.FirstOrDefault();
            if (syntaxRef == null) return null;

            var syntaxNode = syntaxRef.GetSyntax();
            var declarator = syntaxNode as VariableDeclaratorSyntax;
    
            var initializer = declarator.Initializer;
            var creation = initializer?.Value as BaseObjectCreationExpressionSyntax;
        
            return creation.ArgumentList?.Arguments.Single().Expression.ToString();
        }
    }
}