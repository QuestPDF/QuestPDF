using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal static class CompilationExtensions
{
    public static IEnumerable<INamedTypeSymbol> GetMembersRecursively(this INamespaceSymbol namespaceSymbol)
    {
        foreach (var typeSymbol in namespaceSymbol.GetTypeMembers())
            yield return typeSymbol;

        foreach (var nestedNamespace in namespaceSymbol.GetNamespaceMembers())
        foreach (var nestedMember in GetMembersRecursively(nestedNamespace))
            yield return nestedMember;
    }

    public static IEnumerable<INamedTypeSymbol> GetAllQuestPdfTypes(this Compilation compilation)
    {
        return compilation.GlobalNamespace
            .GetNamespaceMembers()
            .Where(x => x.Name.StartsWith("QuestPDF"))
            .SelectMany(x => x.GetMembersRecursively());
    }

    public static IEnumerable<IMethodSymbol> GetAllQuestPdfExtensionMethods(this Compilation compilation)
    {
        return compilation.GetAllQuestPdfTypes()
            .Where(x => !x.IsGenericType)
            .SelectMany(x => x.GetMembers())
            .OfType<IMethodSymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public && x.IsStatic && x.IsExtensionMethod);
    }
}
