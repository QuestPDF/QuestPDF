using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal record OverloadInfo(bool IsOverload, string DisambiguatedName, string OverloadSuffix);

internal static partial class Helpers
{
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2");
        result = Regex.Replace(result, @"([A-Z]+)([A-Z][a-z])", "$1_$2");
        result = Regex.Replace(result, @"([a-zA-Z])(\d)", "$1_$2");
        result = Regex.Replace(result, @"(\d)([a-zA-Z])", "$1_$2");

        return result.ToLowerInvariant();
    }

    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLowerInvariant(input[0]) + input.Substring(1);
    }

    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpperInvariant(input[0]) + input.Substring(1);
    }

    public static string? TryGetDeprecationMessage(this ISymbol symbol)
    {
        return symbol
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == "ObsoleteAttribute")
            ?.ConstructorArguments
            .FirstOrDefault().Value as string;
    }

    public static IEnumerable<INamedTypeSymbol> GetMembersRecursively(this INamespaceSymbol namespaceSymbol)
    {
        foreach (var typeSymbol in namespaceSymbol.GetTypeMembers())
            yield return typeSymbol;

        foreach (var nestedNamespace in namespaceSymbol.GetNamespaceMembers())
        foreach (var nestedMember in GetMembersRecursively(nestedNamespace))
            yield return nestedMember;
    }

    public static IEnumerable<INamedTypeSymbol> FilterSupportedTypes(this IEnumerable<INamedTypeSymbol> typeSymbols)
    {
        return typeSymbols.Where(x => !x.IsGenericType);
    }

    private static readonly string[] ExcludedParameterTypeNames =
    [
        "global::System.Predicate",
        "BoxShadowStyle",
        "TextStyle",
        "IDynamic",
        "Stream",
        "IDynamicElement"
    ];

    public static IEnumerable<IMethodSymbol> FilterSupportedMethods(this IEnumerable<IMethodSymbol> methodSymbols)
    {
        return methodSymbols
            .ExcludeOldObsoleteMethods()
            .Where(x => !x.Parameters.Any(p => p.Type.TypeKind == TypeKind.Array))
            .Where(x => !x.Name.Contains("Component"))
            .Where(x => !x.Parameters.Any(p => !p.Type.IsAction() && !p.Type.IsFunc() && p.Type.TypeKind == TypeKind.Delegate))
            .Where(x => !x.Parameters.Any(p => ExcludedParameterTypeNames.Any(
                phrase => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Contains(phrase))))
            .Where(x => x.Name != "Dispose")
            .Where(x => !(x.Parameters.Skip(1).FirstOrDefault()?.Type?.Name?.Contains("IContainer") ?? false))
            .Where(x => !x.Parameters.Any(p => p.GetAttributes().Any()));
    }

    public static IEnumerable<IMethodSymbol> ExcludeOldObsoleteMethods(this IEnumerable<IMethodSymbol> methodSymbols)
    {
        var oldVersion = new Version(2025, 0);

        return methodSymbols.Where(x => !IsOldObsolete(x));

        bool IsOldObsolete(IMethodSymbol method)
        {
            var obsoleteAttribute = method
                .GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.Name == nameof(ObsoleteAttribute));

            if (obsoleteAttribute == null)
                return false;

            var deprecationMessage = obsoleteAttribute
                .ConstructorArguments
                .FirstOrDefault().Value as string;

            var parser = GetVersionParser().Match(deprecationMessage ?? string.Empty);

            if (!parser.Success)
                return false;

            var yearVersion = parser.Groups["year"].Value;
            var monthVersion = parser.Groups["month"].Value;

            var currentVersion = new Version(int.Parse(yearVersion), int.Parse(monthVersion));
            return currentVersion < oldVersion;
        }
    }

    public static bool InheritsFromOrEquals(this ITypeSymbol type, ITypeSymbol baseType)
    {
        if (SymbolEqualityComparer.Default.Equals(type, baseType))
            return true;

        var current = type.BaseType;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
                return true;

            current = current.BaseType;
        }

        return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, baseType));
    }

    public static bool IsExtensionFor(this IMethodSymbol method, ITypeSymbol targetType)
    {
        if (!method.IsExtensionMethod || method.Parameters.IsEmpty)
            return false;

        var thisParamType = method.Parameters[0].Type;

        if (thisParamType is ITypeParameterSymbol typeParam)
        {
            // Unconstrained generic "this T" applies to all types
            if (!typeParam.ConstraintTypes.Any())
                return true;

            return typeParam.ConstraintTypes.Any(constraint => targetType.InheritsFromOrEquals(constraint));
        }

        return targetType.InheritsFromOrEquals(thisParamType);
    }

    public static string GetInteropTypeName(this ITypeSymbol type)
    {
        return type.TypeKind == TypeKind.Interface ? type.Name.TrimStart('I') : type.Name;
    }

    public static string GetGeneratedClassName(this INamedTypeSymbol type) => type.GetInteropTypeName();

    public static string GetDefaultEnumMemberName(this IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue || parameter.Type.TypeKind != TypeKind.Enum)
            return null;

        return parameter.Type
            .GetMembers()
            .OfType<IFieldSymbol>()
            .FirstOrDefault(x => x.HasConstantValue && x.ConstantValue.Equals(parameter.ExplicitDefaultValue))
            ?.Name;
    }

    public static IReadOnlyList<IParameterSymbol> GetNonThisParameters(this IMethodSymbol method)
    {
        return method.Parameters
            .Skip(method.IsExtensionMethod ? 1 : 0)
            .ToList();
    }

    public static IEnumerable<IParameterSymbol> GetCallbackParameters(this IMethodSymbol method)
    {
        return method.GetNonThisParameters()
            .Where(p => p.Type.IsAction() || p.Type.IsFunc());
    }

    public static string GetCallbackArgumentTypeName(this IParameterSymbol parameter)
    {
        if (parameter.Type is not INamedTypeSymbol { TypeArguments.Length: > 0 } delegateType)
            return null;

        return delegateType.TypeArguments[0].GetInteropTypeName();
    }

    public static Dictionary<IMethodSymbol, OverloadInfo> ComputeOverloads(this IReadOnlyList<IMethodSymbol> methods)
    {
        var result = new Dictionary<IMethodSymbol, OverloadInfo>(SymbolEqualityComparer.Default);

        foreach (var group in methods.GroupBy(m => m.Name))
        {
            var groupMethods = group.ToList();

            if (groupMethods.Count == 1)
            {
                result[groupMethods[0]] = new OverloadInfo(false, groupMethods[0].Name, string.Empty);
                continue;
            }

            var entries = groupMethods.Select(m => (Method: m, Suffix: GenerateOverloadSuffix(m))).ToList();

            foreach (var suffixGroup in entries.GroupBy(x => x.Suffix))
            {
                var collisions = suffixGroup.ToList();

                for (int i = 0; i < collisions.Count; i++)
                {
                    var suffix = collisions.Count > 1 ? $"{collisions[i].Suffix}_{i + 1}" : collisions[i].Suffix;
                    result[collisions[i].Method] = new OverloadInfo(true, collisions[i].Method.Name + suffix, suffix);
                }
            }
        }

        return result;
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

    [GeneratedRegex(@"(?<year>20\d{2})\.(?<month>0?[1-9]|1[0-2])")]
    private static partial Regex GetVersionParser();
}
