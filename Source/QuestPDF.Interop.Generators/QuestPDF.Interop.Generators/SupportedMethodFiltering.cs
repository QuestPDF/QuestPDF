using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal static class SupportedMethodFiltering
{
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
}
