using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal static class NativeTypeMapping
{
    public static string GetDeterministicHash(this string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(bytes);
        return string.Concat(hashBytes.Take(4).Select(b => b.ToString("x2")));
    }

    public static string ExtractNativeMethodHash(this string nativeEntryPoint)
    {
        var lastSeparator = nativeEntryPoint.LastIndexOf("__");
        return lastSeparator >= 0 && lastSeparator < nativeEntryPoint.Length - 2
            ? nativeEntryPoint.Substring(lastSeparator + 2)
            : nativeEntryPoint.GetHashCode().ToString("x8");
    }

    public static string GetNativeMethodName(this IMethodSymbol methodSymbol, string targetTypeName)
    {
        var hash = methodSymbol.ToDisplayString().GetDeterministicHash();
        return $"questpdf__{targetTypeName.ToSnakeCase()}__{methodSymbol.Name.ToSnakeCase()}__{hash}";
    }

    public static string GetNativeParameterType(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.SpecialType == SpecialType.System_Void)
            return "void";

        if (typeSymbol.TypeKind == TypeKind.Enum)
            return "int32_t";

        if (typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Contains("QuestPDF.Infrastructure.Color"))
            return "uint32_t";

        if (typeSymbol.SpecialType == SpecialType.System_String)
            return "const char*";

        if (typeSymbol.TypeKind is TypeKind.Class or TypeKind.Interface)
            return "void*";

        if ((typeSymbol.IsAction() || typeSymbol.IsFunc()) && typeSymbol is INamedTypeSymbol delegateSymbol)
        {
            var nativeTypes = delegateSymbol.TypeArguments.Select(x => x.GetNativeParameterType());
            if (typeSymbol.IsAction())
                nativeTypes = nativeTypes.Append("void");
            return $"delegate* unmanaged[Cdecl]<{string.Join(", ", nativeTypes)}>";
        }

        return typeSymbol.SpecialType switch
        {
            SpecialType.System_Int32 => "int32_t",
            SpecialType.System_UInt32 => "uint32_t",
            SpecialType.System_Int64 => "int64_t",
            SpecialType.System_UInt64 => "uint64_t",
            SpecialType.System_Int16 => "int16_t",
            SpecialType.System_UInt16 => "uint16_t",
            SpecialType.System_Byte => "uint8_t",
            SpecialType.System_SByte => "int8_t",
            SpecialType.System_Single => "float",
            SpecialType.System_Double => "double",
            SpecialType.System_Boolean => "uint8_t",
            SpecialType.System_Char => "uint16_t",
            SpecialType.System_IntPtr or SpecialType.System_UIntPtr => "void*",
            _ => "void*"
        };
    }

    public static string GetCallbackTypedefName(this ITypeSymbol typeSymbol, IMethodSymbol containingMethod)
    {
        var hash = containingMethod.ToDisplayString().GetDeterministicHash();

        if (typeSymbol.IsAction() && typeSymbol is INamedTypeSymbol actionSymbol)
        {
            if (actionSymbol.TypeArguments.Length == 0)
                return $"voidCallback_{hash}";

            var argNames = actionSymbol.TypeArguments.Select(x => x.GetInteropTypeName().ToSnakeCase());
            return $"{string.Join("_", argNames)}_callback_{hash}";
        }

        if (typeSymbol.IsFunc() && typeSymbol is INamedTypeSymbol funcSymbol)
        {
            var argNames = funcSymbol.TypeArguments.SkipLast(1).Select(x => x.GetInteropTypeName().ToSnakeCase());
            var returnName = funcSymbol.TypeArguments.Last().GetInteropTypeName().ToSnakeCase();
            return $"{string.Join("_", argNames)}_{returnName}_func_{hash}";
        }

        return "unknown_callback";
    }

    public static string GetCallbackTypedefDefinition(this ITypeSymbol typeSymbol, IMethodSymbol containingMethod)
    {
        var typedefName = typeSymbol.GetCallbackTypedefName(containingMethod);

        if ((typeSymbol.IsAction() || typeSymbol.IsFunc()) && typeSymbol is INamedTypeSymbol { TypeArguments.Length: > 0 } delegateSymbol)
        {
            var isFunc = typeSymbol.IsFunc();
            var paramTypes = (isFunc ? delegateSymbol.TypeArguments.SkipLast(1) : delegateSymbol.TypeArguments)
                .Select(x => x.GetNativeParameterType());
            var returnType = isFunc ? delegateSymbol.TypeArguments.Last().GetNativeParameterType() : "void";
            return $"typedef {returnType} (*{typedefName})({string.Join(", ", paramTypes)});";
        }

        return $"typedef void (*{typedefName})();";
    }

    public static IEnumerable<(string TypedefName, string TypedefDefinition)> GetCallbackTypedefs(this IEnumerable<IMethodSymbol> methods)
    {
        return methods.SelectMany(method => method.Parameters
            .Where(p => p.Type.IsAction() || p.Type.IsFunc())
            .Select(p => (p.Type.GetCallbackTypedefName(method), p.Type.GetCallbackTypedefDefinition(method))));
    }

    public static string GetCHeaderDefinition(this IMethodSymbol methodSymbol, string targetTypeName)
    {
        var resultType = methodSymbol.ReturnType.GetNativeParameterType();
        var methodName = methodSymbol.GetNativeMethodName(targetTypeName);

        var parameters = methodSymbol.Parameters.Select(x =>
        {
            var type = x.Type.IsAction() || x.Type.IsFunc()
                ? x.Type.GetCallbackTypedefName(methodSymbol)
                : x.Type.GetNativeParameterType();
            return $"{type} {x.Name}";
        });

        if (!methodSymbol.IsExtensionMethod && !methodSymbol.IsStatic)
            parameters = parameters.Prepend("void* target");

        return $"{resultType} {methodName}({string.Join(", ", parameters)});";
    }
}
