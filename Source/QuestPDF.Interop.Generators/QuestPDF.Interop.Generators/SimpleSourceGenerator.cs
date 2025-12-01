using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal class SimpleSourceGenerator(string targetNamespace) : ObjectSourceGeneratorBase
{
    protected override IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation)
    {
        return compilation
            .GetTypeByMetadataName(targetNamespace)
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .FilterSupportedMethods()
            .ToList();
    }

    protected override INamedTypeSymbol GetTargetType(Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(targetNamespace);
    }
}