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
    
    public static string GetManagedMethodName(this IMethodSymbol methodSymbol)
    {
        var targetType = methodSymbol.IsExtensionMethod 
            ? methodSymbol.Parameters.First().Type 
            : methodSymbol.ContainingType;

        var isInterface = targetType.TypeKind == TypeKind.Interface;
        var targetTypeName = isInterface ? targetType.Name.TrimStart('I') : targetType.Name; 
        
        return $"{targetTypeName}_{methodSymbol.Name}";
    }
    
    public static string GetInteropResultType(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.TypeKind == TypeKind.Class)
            return "IntPtr";
        
        if (typeSymbol.TypeKind == TypeKind.Interface)
            return "IntPtr";
        
        return typeSymbol.ToString();
    }
    
    public static string GetInteropMethodParameterType(this ITypeSymbol typeSymbol)
    {
        var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (typeSymbol.TypeKind == TypeKind.Enum)
            return "int";

        if (typeSymbol.TypeKind == TypeKind.Class)
            return "IntPtr";
        
        if (typeSymbol.TypeKind == TypeKind.Interface)
            return "IntPtr";
        
        return typeName;
    }
    
    public static string GetNativeParameterType(this ITypeSymbol typeSymbol)
    {
        var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (typeSymbol.SpecialType == SpecialType.System_Void)
            return "void";
        
        if (typeSymbol.TypeKind == TypeKind.Enum)
            return "int32_t";
        
        return typeName switch
        {
            "int" or "System.Int32" => "int32_t",
            "uint" or "System.UInt32" => "uint32_t",
            "long" or "System.Int64" => "int64_t",
            "ulong" or "System.UInt64" => "uint64_t",
            "short" or "System.Int16" => "int16_t",
            "ushort" or "System.UInt16" => "uint16_t",
            "byte" or "System.Byte" => "uint8_t",
            "sbyte" or "System.SByte" => "int8_t",
            "float" or "System.Single" => "float",
            "double" or "System.Double" => "double",
            "bool" or "System.Boolean" => "uint8_t",
            "char" or "System.Char" => "uint16_t",
            "string" or "System.String" => "const char*",
            "System.IntPtr" => "nint",
            "System.UIntPtr" => "nuint",
            _ when typeSymbol.TypeKind == TypeKind.Pointer => "nint",
            _ when typeSymbol.TypeKind == TypeKind.Class || typeSymbol.TypeKind == TypeKind.Interface => "nint",
            _ => "nint" // Default for unknown types
        };
    }
}