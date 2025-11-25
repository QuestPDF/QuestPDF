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
        var model = GetTemplateModel(compilation);
        return ScribanTemplateLoader.RenderTemplate("Python.Colors", model);
    }

    public string GenerateJavaCode(Compilation compilation)
    {
        var model = GetTemplateModel(compilation);
        return ScribanTemplateLoader.RenderTemplate("Java.Colors", model);
    }
    
    public string GenerateTypeScriptCode(Compilation compilation)
    {
        var model = GetTemplateModel(compilation);
        return ScribanTemplateLoader.RenderTemplate("TypeScript.Colors", model);
    }

    #region Shared

    class TemplateModel
    {
        public IEnumerable<Color> BasicColors { get; set; }
        public IEnumerable<ColorGroup> ColorGroups { get; set; }
        
        public class Color
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class ColorGroup
        {
            public string Name { get; set; }
            public IEnumerable<Color> Colors { get; set; }
        }
    }

    private TemplateModel GetTemplateModel(Compilation compilation)
    {
        var colorType = compilation.GetTypeByMetadataName("QuestPDF.Infrastructure.Color");
        var colorClass = compilation.GetTypeByMetadataName("QuestPDF.Helpers.Colors")!;

        return new TemplateModel
        {
            BasicColors = GetColorDefinitions(colorClass),
            ColorGroups = GetColorGroups()
        };

        IEnumerable<TemplateModel.ColorGroup> GetColorGroups()
        {
            return colorClass
                .GetTypeMembers()
                .Where(t => t.TypeKind == TypeKind.Class)
                .Select(t => new TemplateModel.ColorGroup
                {
                    Name = t.Name, 
                    Colors = GetColorDefinitions(t)
                });
        }

        IEnumerable<TemplateModel.Color> GetColorDefinitions(INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol
                .GetMembers()
                .Where(x => x.IsStatic)
                .OfType<IFieldSymbol>()
                .Where(x => SymbolEqualityComparer.Default.Equals(x.Type, colorType))
                .Select(x => new TemplateModel.Color
                {
                    Name = x.Name,
                    Value = GetColorValue(x)
                });
        }

        static string GetColorValue(IFieldSymbol fieldSymbol)
        {
            var syntaxRef = fieldSymbol.DeclaringSyntaxReferences.Single();
            var syntaxNode = syntaxRef.GetSyntax();
            var declarator = syntaxNode as VariableDeclaratorSyntax;
    
            var initializer = declarator.Initializer;
            var creation = initializer?.Value as BaseObjectCreationExpressionSyntax;
        
            return creation.ArgumentList?.Arguments.Single().Expression.ToString();
        }
    }

    #endregion
}