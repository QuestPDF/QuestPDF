using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal class SimpleSourceGenerator(Type targetType) : ObjectSourceGeneratorBase(targetType)
{
    protected override IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation)
    {
        return GetTargetType(compilation).GetMembers()
            .OfType<IMethodSymbol>()
            .Where(x => !ExcludeMembers.Any(x.Name.Contains))
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .FilterSupportedMethods()
            .ToList();
    }
}
