using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal record OverloadInfo(bool IsOverload, string DisambiguatedName, string OverloadSuffix);

internal static class OverloadResolution
{
    public static Dictionary<IMethodSymbol, OverloadInfo> ComputeOverloads(this IReadOnlyList<IMethodSymbol> methods)
    {
        return methods
            .GroupBy(m => m.Name)
            .SelectMany(ProcessNameGroup)
            .ToDictionary(x => x.Method, x => x.Info);

        static IEnumerable<(IMethodSymbol Method, OverloadInfo Info)> ProcessNameGroup(IGrouping<string, IMethodSymbol> group)
        {
            var groupMethods = group.ToList();

            if (groupMethods.Count == 1)
                return [(groupMethods[0], new OverloadInfo(false, groupMethods[0].Name, string.Empty))];

            return groupMethods
                .Select(m => (Method: m, Suffix: GenerateOverloadSuffix(m)))
                .GroupBy(x => x.Suffix)
                .SelectMany(ResolveSuffixCollisions);
        }

        static IEnumerable<(IMethodSymbol Method, OverloadInfo Info)> ResolveSuffixCollisions(IGrouping<string, (IMethodSymbol Method, string Suffix)> suffixGroup)
        {
            var collisions = suffixGroup.ToList();

            return collisions.Select((entry, i) =>
            {
                var suffix = collisions.Count > 1 ? $"{entry.Suffix}_{i + 1}" : entry.Suffix;
                return (entry.Method, new OverloadInfo(true, entry.Method.Name + suffix, suffix));
            });
        }
    }

    private static string GenerateOverloadSuffix(IMethodSymbol method)
    {
        var parameters = method.GetNonThisParameters();

        if (parameters.Count == 0)
            return "_NoArgs";

        var parts = parameters.Select(GetTypeShortName);
        return "_" + string.Join("_", parts);
    }

    private static string GetTypeShortName(IParameterSymbol parameter)
    {
        var kind = parameter.Type.GetInteropTypeKind();

        if (kind is InteropTypeKind.Enum or InteropTypeKind.Class or InteropTypeKind.Interface)
            return parameter.Type.GetInteropTypeName();

        return kind.ToString();
    }
}
