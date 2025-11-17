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

    class EnumTemplateModel
    {
        public string Name { get; set; }
        public IEnumerable<EnumMember> Members { get; set; }

        public class EnumMember
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }

    public string GeneratePythonCode(Compilation compilation)
    {
        var model = new
        {
            Enums = GetAllPublicEnumsFromCompilation(compilation).Select(Map)
        };
        
        return ScribanTemplateLoader.LoadTemplate("Enum.py").Render(model);
    }

    private static EnumTemplateModel Map(INamedTypeSymbol symbol)
    {
        return new EnumTemplateModel
        {
            Name = symbol.Name,
            Members = symbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(x => x.HasConstantValue)
                .Select(x => new EnumTemplateModel.EnumMember
                {
                    Name = x.Name.ToPythonEnumMemberName(), 
                    Value = System.Convert.ToInt32(x.ConstantValue)
                })
        };
    }
    
    private IEnumerable<INamedTypeSymbol> GetAllPublicEnumsFromCompilation(Compilation compilation)
    {
        return compilation
            .GlobalNamespace
            .GetNamespaceMembers()
            .Where(x => x.Name.StartsWith("QuestPDF"))
            .SelectMany(x => x.GetMembersRecursively())
            .Where(x => x.TypeKind == TypeKind.Enum && x.DeclaredAccessibility == Accessibility.Public);
    }
}