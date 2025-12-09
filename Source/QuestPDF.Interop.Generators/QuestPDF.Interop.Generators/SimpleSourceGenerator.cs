using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal class SimpleSourceGenerator(Type targetType) : ObjectSourceGeneratorBase
{
    public ICollection<string> ExcludeMembers { get; set; } = Array.Empty<string>();
    
    protected override IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation)
    {
        return compilation
            .GetTypeByMetadataName(targetType.FullName)
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(x => !ExcludeMembers.Any(x.Name.Contains))
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .FilterSupportedMethods()
            .ToList();
    }

    protected override INamedTypeSymbol GetTargetType(Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(targetType.FullName);
    }
}