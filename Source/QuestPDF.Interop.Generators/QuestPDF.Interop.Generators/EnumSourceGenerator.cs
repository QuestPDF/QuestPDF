using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

public class EnumSourceGenerator() : IInteropSourceGenerator
{
    public string GenerateCSharpCode(Compilation compilation)
    {
        return string.Empty;
    }
    
    public string GeneratePythonCode(Compilation compilation)
    {
        var model = GetTemplateModel(compilation);
        return FluidTemplateLoader.RenderTemplate("Python.Enum", model);
    }

    public string GenerateJavaCode(Compilation compilation)
    {
        var model = GetTemplateModel(compilation);
        return FluidTemplateLoader.RenderTemplate("Java.Enum", model);
    }

    public string GenerateTypeScriptCode(Compilation compilation)
    {
        var model = GetTemplateModel(compilation);
        return FluidTemplateLoader.RenderTemplate("TypeScript.Enum", model);
    }
    
    #region Shared
    
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

    private TemplateModel GetTemplateModel(Compilation compilation)
    {
        return new TemplateModel
        {
            Enums = GetAllPublicEnumsFromCompilation(compilation).Select(Map)
        };
    }

    private static TemplateModel.Enum Map(INamedTypeSymbol symbol)
    {
        return new TemplateModel.Enum
        {
            Name = symbol.Name,
            Members = GetEnumMembers(symbol)
        };
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
    
    #endregion
}