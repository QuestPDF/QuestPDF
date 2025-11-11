using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal static class Helpers
{
    private static readonly Regex SnakeCaseRegex = new("(?<!^)([A-Z])", RegexOptions.Compiled);
    
    public static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return SnakeCaseRegex.Replace(input, "_$1").ToLowerInvariant();
    }
    
    public static string GetNativeMethodName(this IMethodSymbol methodSymbol)
    {
        var targetType = methodSymbol.IsExtensionMethod 
            ? methodSymbol.Parameters.First().Type 
            : methodSymbol.ContainingType;

        var isInterface = targetType.TypeKind == TypeKind.Interface;
        var targetTypeName = isInterface ? targetType.Name.TrimStart('I') : targetType.Name; 
        
        return $"questpdf__{ToSnakeCase(targetTypeName)}__{ToSnakeCase(methodSymbol.Name)}__{methodSymbol.GetHashCode()}";
    }
}