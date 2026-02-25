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

        return char.ToLowerInvariant(input[0]) + input[1..];
    }

    public static string ToPascalCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpperInvariant(input[0]) + input[1..];
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
        return methodSymbols.ExcludeOldObsoleteMethods().Where(IsSupported);

        static bool IsSupported(IMethodSymbol method)
        {
            if (method.Name is "Dispose" || method.Name.Contains("Component"))
                return false;

            if (method.Parameters.Any(p => p.Type.TypeKind == TypeKind.Array))
                return false;

            if (method.Parameters.Any(p => p.Type.TypeKind == TypeKind.Delegate && !p.Type.IsAction() && !p.Type.IsFunc()))
                return false;

            if (method.Parameters.Any(p => p.GetAttributes().Any()))
                return false;

            if (method.Parameters.Skip(1).FirstOrDefault()?.Type?.Name?.Contains("IContainer") ?? false)
                return false;

            if (method.Parameters.Any(HasExcludedParameterType))
                return false;

            return true;
        }

        static bool HasExcludedParameterType(IParameterSymbol parameter)
        {
            var typeName = parameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return ExcludedParameterTypeNames.Any(typeName.Contains);
        }
    }

    public static IEnumerable<IMethodSymbol> ExcludeOldObsoleteMethods(this IEnumerable<IMethodSymbol> methodSymbols)
    {
        return methodSymbols.Where(method => !IsOldObsolete(method));

        static bool IsOldObsolete(IMethodSymbol method)
        {
            var deprecationMessage = method.TryGetDeprecationMessage();

            if (deprecationMessage is null)
                return false;

            var match = Regex.Match(deprecationMessage, @"(?<year>20\d{2})\.(?<month>0?[1-9]|1[0-2])");

            if (!match.Success)
                return false;

            var year = int.Parse(match.Groups["year"].Value);
            return year < 2025;
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

    public static string? GetDefaultEnumMemberName(this IParameterSymbol parameter)
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
        return method
            .GetNonThisParameters()
            .Where(p => p.Type.IsAction() || p.Type.IsFunc());
    }

    public static string? GetCallbackArgumentTypeName(this IParameterSymbol parameter)
    {
        if (parameter.Type is not INamedTypeSymbol { TypeArguments.Length: > 0 } delegateType)
            return null;

        return delegateType.TypeArguments[0].GetInteropTypeName();
    }

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
