using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

// Generates interop code for all public extension methods targeting the specified container interface.
// Use targetInterfaceName/targetTypeMetadataName to target different container interfaces
// (e.g., "IContainer"/"QuestPDF.Infrastructure.IContainer" or "ITableCellContainer"/"QuestPDF.Elements.Table.ITableCellContainer").
internal class ContainerSourceGenerator(
    string targetInterfaceName = "IContainer",
    string targetTypeMetadataName = "QuestPDF.Infrastructure.IContainer") : ObjectSourceGeneratorBase
{
    public ICollection<string> ExcludeMembers { get; set; } = Array.Empty<string>();
    
    protected override IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation)
    {
        return compilation
            .GlobalNamespace
            .GetNamespaceMembers()
            .Where(x => x.Name.StartsWith("QuestPDF"))
            .SelectMany(x => x.GetMembersRecursively())
            .FilterSupportedTypes()
            .SelectMany(x => x.GetMembers())
            .OfType<IMethodSymbol>()
            .Where(x => !ExcludeMembers.Any(x.ToDisplayString().Contains))
            .FilterSupportedMethods()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public && x.IsStatic && x.IsExtensionMethod)
            .Where(x => x.Parameters.First().Type.Name.Contains(targetInterfaceName));
    }

    protected override INamedTypeSymbol GetTargetType(Compilation compilation)
    {
        return compilation.GetTypeByMetadataName(targetTypeMetadataName);
    }
}