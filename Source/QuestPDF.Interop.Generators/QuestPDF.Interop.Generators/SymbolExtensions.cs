using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal static class SymbolExtensions
{
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
}
