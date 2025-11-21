using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal class DescriptorSourceGenerator(string targetNamespace) : ObjectSourceGeneratorBase
{
    protected override IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation)
    {
        var targetType = compilation
            .GetTypeByMetadataName(targetNamespace);

        var extensionMethods = compilation
            .GlobalNamespace
            .GetNamespaceMembers()
            .Where(x => x.Name.StartsWith("QuestPDF"))
            .SelectMany(x => x.GetMembersRecursively())
            .FilterSupportedTypes()
            .SelectMany(x => x.GetMembers())
            .OfType<IMethodSymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public && x.IsStatic && x.IsExtensionMethod)
            .ToList();
        
        var genericMethods = extensionMethods
            .Where(m => m.IsExtensionFor(targetType))
            .ToList();

        var implicitMethods = targetType
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.DeclaredAccessibility == Accessibility.Public);

        return implicitMethods
            .Concat(genericMethods)
            .FilterSupportedMethods();
    }

    protected override INamedTypeSymbol GetTargetType(Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(targetNamespace);
    }
}