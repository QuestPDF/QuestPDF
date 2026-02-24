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
        var lastUnderscore = nativeEntryPoint.LastIndexOf("__");
        if (lastUnderscore >= 0 && lastUnderscore < nativeEntryPoint.Length - 2)
        {
            return nativeEntryPoint.Substring(lastUnderscore + 2);
        }
        return nativeEntryPoint.GetHashCode().ToString("x8");
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

        if (typeSymbol.TypeKind == TypeKind.Class)
            return "void*";

        if (typeSymbol.TypeKind == TypeKind.Interface)
            return "void*";

        if (typeSymbol.IsAction() && typeSymbol is INamedTypeSymbol actionSymbol)
        {
            var genericTypes = actionSymbol
                .TypeArguments
                .Select(x => x.GetNativeParameterType())
                .Append("void");

            var genericTypesString = string.Join(", ", genericTypes);
            return $"delegate* unmanaged[Cdecl]<{genericTypesString}>";
        }

        if (typeSymbol.IsFunc() && typeSymbol is INamedTypeSymbol funcSymbol)
        {
            var genericTypes = funcSymbol
                .TypeArguments
                .Select(x => x.GetNativeParameterType());

            var genericTypesString = string.Join(", ", genericTypes);
            return $"delegate* unmanaged[Cdecl]<{genericTypesString}>";
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
        var prefix = containingMethod.ToDisplayString().GetDeterministicHash();

        if (typeSymbol.IsAction() && typeSymbol is INamedTypeSymbol actionSymbol)
        {
            var argumentTypes = actionSymbol.TypeArguments;
            if (argumentTypes.Length == 0)
                return $"voidCallback_{prefix}";

            var argumentNames = argumentTypes.Select(x =>
            {
                var typeName = x.TypeKind == TypeKind.Interface ? x.Name.TrimStart('I') : x.Name;
                return typeName.ToSnakeCase();
            });
            return $"{string.Join("_", argumentNames)}_callback_{prefix}";
        }

        if (typeSymbol.IsFunc() && typeSymbol is INamedTypeSymbol funcSymbol)
        {
            var argumentTypes = funcSymbol.TypeArguments.Take(funcSymbol.TypeArguments.Length - 1);
            var returnType = funcSymbol.TypeArguments.Last();

            var argumentNames = argumentTypes.Select(x =>
            {
                var typeName = x.TypeKind == TypeKind.Interface ? x.Name.TrimStart('I') : x.Name;
                return typeName.ToSnakeCase();
            });

            var returnTypeName = returnType.TypeKind == TypeKind.Interface
                ? returnType.Name.TrimStart('I').ToSnakeCase()
                : returnType.Name.ToSnakeCase();

            return $"{string.Join("_", argumentNames)}_{returnTypeName}_func_{prefix}";
        }

        return "unknown_callback";
    }

    public static string GetCallbackTypedefDefinition(this ITypeSymbol typeSymbol, IMethodSymbol containingMethod)
    {
        var typedefName = typeSymbol.GetCallbackTypedefName(containingMethod);

        if (typeSymbol.IsAction() && typeSymbol is INamedTypeSymbol actionSymbol)
        {
            var parameters = actionSymbol
                .TypeArguments
                .Select(x => x.GetNativeParameterType());

            var parametersString = string.Join(", ", parameters);
            return $"typedef void (*{typedefName})({parametersString});";
        }

        if (typeSymbol.IsFunc() && typeSymbol is INamedTypeSymbol funcSymbol)
        {
            var parameters = funcSymbol
                .TypeArguments
                .Take(funcSymbol.TypeArguments.Length - 1)
                .Select(x => x.GetNativeParameterType());

            var returnType = funcSymbol.TypeArguments.Last().GetNativeParameterType();
            var parametersString = string.Join(", ", parameters);
            return $"typedef {returnType} (*{typedefName})({parametersString});";
        }

        return $"typedef void (*{typedefName})();";
    }

    public static IEnumerable<(string TypedefName, string TypedefDefinition)> GetCallbackTypedefs(this IEnumerable<IMethodSymbol> methods)
    {
        foreach (var methodSymbol in methods)
        {
            var results = methodSymbol.Parameters
                .Where(p => p.Type.IsAction() || p.Type.IsFunc())
                .Select(p => p.Type)
                .Select(p => (p.GetCallbackTypedefName(methodSymbol), p.GetCallbackTypedefDefinition(methodSymbol)));

            foreach (var valueTuple in results)
                yield return valueTuple;
        }
    }

    public static string GetCHeaderDefinition(this IMethodSymbol methodSymbol, string targetTypeName)
    {
        var resultType = methodSymbol.ReturnType.GetNativeParameterType();
        var methodName = methodSymbol.GetNativeMethodName(targetTypeName);

        var parameters = methodSymbol
            .Parameters
            .Select(x =>
            {
                if (x.Type.IsAction() || x.Type.IsFunc())
                    return $"{x.Type.GetCallbackTypedefName(methodSymbol)} {x.Name}";

                return $"{x.Type.GetNativeParameterType()} {x.Name}";
            });

        if (!methodSymbol.IsExtensionMethod && !methodSymbol.IsStatic)
            parameters = parameters.Prepend("void* target");

        var parametersString = string.Join(", ", parameters);

        return $"{resultType} {methodName}({parametersString});";
    }
}
