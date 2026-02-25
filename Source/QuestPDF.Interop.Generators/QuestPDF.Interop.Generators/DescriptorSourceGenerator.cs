using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal class DescriptorSourceGenerator(Type targetType) : ObjectSourceGeneratorBase(targetType)
{
    public bool IncludeInheritedExtensionMethods { get; init; }

    protected override IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation)
    {
        var resolvedType = GetTargetType(compilation);

        var implicitMethods = resolvedType.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.DeclaredAccessibility == Accessibility.Public);

        var extensionMethods = IncludeInheritedExtensionMethods
            ? compilation.GetAllQuestPdfExtensionMethods().Where(m => m.IsExtensionFor(resolvedType))
            : [];

        return implicitMethods.Concat(extensionMethods)
            .Where(x => !ExcludeMembers.Any(x.Name.Contains))
            .FilterSupportedMethods();
    }
}
