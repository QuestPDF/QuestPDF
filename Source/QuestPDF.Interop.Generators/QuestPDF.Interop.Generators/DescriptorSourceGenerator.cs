using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal class DescriptorSourceGenerator(string targetNamespace) : ObjectSourceGeneratorBase
{
    protected override IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation)
    {
        return compilation
            .GetTypeByMetadataName(targetNamespace)
            .GetMembers()
            .OfType<IMethodSymbol>()
            .FilterSupportedMethods()
            .Where(m => m.DeclaredAccessibility == Accessibility.Public);
    }
}