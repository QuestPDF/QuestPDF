using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal static class Helpers
{
    private static readonly Regex SnakeCaseRegex = new("(?<!^)([A-Z])", RegexOptions.Compiled);
    
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return SnakeCaseRegex.Replace(input, "_$1").ToLowerInvariant();
    }
    
    public static string ToPythonEnumMemberName(this string name)
    {
        return name.ToSnakeCase().ToUpper();
    }
    
    public static string GetNativeMethodName(this IMethodSymbol methodSymbol, string targetTypeName)
    {
        var hash = methodSymbol.ToDisplayString().GetDeterministicHash();
        return $"questpdf__{ToSnakeCase(targetTypeName)}__{ToSnakeCase(methodSymbol.Name)}__{hash}";
    }
    
    public static string GetDeterministicHash(this string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hashBytes = sha256.ComputeHash(bytes);
        return string.Concat(hashBytes.Take(4).Select(b => b.ToString("x2")));
    }
    
    public static string GetManagedMethodName(this IMethodSymbol methodSymbol, string targetTypeName)
    {
        var hash = methodSymbol.ToDisplayString().GetDeterministicHash();
        return $"{targetTypeName}_{methodSymbol.Name}_{hash}";
    }
    
    public static string GetInteropResultType(this IMethodSymbol methodSymbol)
    {
        if (methodSymbol.IsGenericMethod && methodSymbol.ReturnType.TypeKind == TypeKind.TypeParameter)
            return "IntPtr";
        
        if (methodSymbol.ReturnType.TypeKind == TypeKind.Class)
            return "IntPtr";
        
        if (methodSymbol.ReturnType.TypeKind == TypeKind.Interface)
            return "IntPtr";
        
        return methodSymbol.ReturnType.ToString();
    }
    
    public static string GetInteropMethodParameterType(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.TypeKind == TypeKind.TypeParameter)
            return "IntPtr";
        
        if (typeSymbol.TypeKind == TypeKind.Enum)
            return "int";

        if (typeSymbol.TypeKind == TypeKind.Class)
            return "IntPtr";
        
        if (typeSymbol.TypeKind == TypeKind.Interface)
            return "IntPtr";
        
        return typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
    
    public static string GetNativeParameterType(this ITypeSymbol typeSymbol)
    {
        var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (typeSymbol.SpecialType == SpecialType.System_Void)
            return "void";
        
        if (typeSymbol.TypeKind == TypeKind.Enum)
            return "int32_t";
        
        if (typeName.Contains("QuestPDF.Infrastructure.Color"))
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
            "System.IntPtr" => "void*",
            "System.UIntPtr" => "void*",
            _ when typeSymbol.TypeKind == TypeKind.Pointer => "void*",
            _ when typeSymbol.TypeKind == TypeKind.Class || typeSymbol.TypeKind == TypeKind.Interface => "void*",
            _ => "void*" // Default for unknown types
        };
    }
    
    public static string GetPythonParameterType(this ITypeSymbol typeSymbol)
    {
        var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (typeSymbol.SpecialType == SpecialType.System_Void)
            return "None";

        if (typeSymbol.TypeKind == TypeKind.Enum)
            return typeSymbol.Name;

        if (typeSymbol.IsAction() && typeSymbol is INamedTypeSymbol actionSymbol)
        {
            var callbackArgument = actionSymbol.TypeArguments.FirstOrDefault();
            if (callbackArgument != null)
            {
                var argumentTypeName = callbackArgument.TypeKind == TypeKind.Interface
                    ? callbackArgument.Name.TrimStart('I')
                    : callbackArgument.Name;
                return $"Callable[['{argumentTypeName}'], None]";
            }
            return "Callable[[], None]";
        }

        if (typeSymbol.IsFunc() && typeSymbol is INamedTypeSymbol funcSymbol)
        {
            var arguments = funcSymbol.TypeArguments.Take(funcSymbol.TypeArguments.Length - 1);
            var returnType = funcSymbol.TypeArguments.Last();

            var argumentTypes = string.Join(", ", arguments.Select(arg =>
            {
                var argTypeName = arg.TypeKind == TypeKind.Interface
                    ? arg.Name.TrimStart('I')
                    : arg.Name;
                return argTypeName;
            }));

            var returnTypeName = returnType.TypeKind == TypeKind.Interface
                ? returnType.Name.TrimStart('I')
                : returnType.Name;

            return $"Callable[['{argumentTypes}'], '{returnTypeName}']";
        }

        if (typeSymbol.TypeKind == TypeKind.Class && typeSymbol.SpecialType != SpecialType.System_String && typeSymbol.SpecialType != SpecialType.System_Object)
            return typeSymbol.Name;

        if (typeSymbol.TypeKind == TypeKind.Interface)
            return typeSymbol.Name.TrimStart('I');

        return typeName switch
        {
            "int" or "System.Int32" => "int",
            "uint" or "System.UInt32" => "int",
            "long" or "System.Int64" => "int",
            "ulong" or "System.UInt64" => "int",
            "short" or "System.Int16" => "int",
            "ushort" or "System.UInt16" => "int",
            "byte" or "System.Byte" => "int",
            "sbyte" or "System.SByte" => "int",
            "float" or "System.Single" => "float",
            "double" or "System.Double" => "float",
            "bool" or "System.Boolean" => "bool",
            "char" or "System.Char" => "str",
            "string" or "System.String" => "str",
            "System.IntPtr" => "int",
            "System.UIntPtr" => "int",
            _ => "Any" // Default for unknown types
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
        
        // var callbackTypes = methods
        //     .SelectMany(m => m.Parameters)
        //     .Where(p => p.Type.IsAction() || p.Type.IsFunc())
        //     .Select(p => p.Type);
        //
        // return callbackTypes.Select(t => (t.GetCallbackTypedefName(), t.GetCallbackTypedefDefinition()));
    }

    public static string GetCHeaderDefinition(this IMethodSymbol methodSymbol, string targetTypeName)
    {
        var resultType = methodSymbol.ReturnType.GetNativeParameterType();
        var methodName = methodSymbol.GetNativeMethodName(targetTypeName);

        var parameters = methodSymbol
            .Parameters
            .Select(x =>
            {
                // Use typedef name for callbacks instead of inline function pointer
                if (x.Type.IsAction() || x.Type.IsFunc())
                    return $"{x.Type.GetCallbackTypedefName(methodSymbol)} {x.Name}";

                return $"{GetNativeParameterType(x.Type)} {x.Name}";
            });

        if (!methodSymbol.IsExtensionMethod)
            parameters = parameters.Prepend("void* target");

        var parametersString = string.Join(", ", parameters);

        return $"{resultType} {methodName}({parametersString});";
    }

    public static string? TryGetDeprecationMessage(this ISymbol symbol)
    {
        return symbol
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == "ObsoleteAttribute")
            ?.ConstructorArguments
            .FirstOrDefault().Value as string;
    }
    
    public static bool IsAction(this ITypeSymbol typeSymbol)
    {
        return typeSymbol
            .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .StartsWith("global::System.Action");
    }
    
    public static bool IsFunc(this ITypeSymbol typeSymbol)
    {
        return typeSymbol
            .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .StartsWith("global::System.Func");
    }
    
    public static bool HasSimpleSingleActionCallback(this IMethodSymbol methodSymbol)
    {
        var parameters = methodSymbol
            .Parameters
            .AsEnumerable();

        if (methodSymbol.IsExtensionMethod)
            parameters = parameters.Skip(1);

        if (parameters.Count() > 1)
            return false;

        return parameters.Single().Type.IsAction();
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
    
    public static IEnumerable<IMethodSymbol> FilterSupportedMethods(this IEnumerable<IMethodSymbol> methodSymbols)
    {
        return methodSymbols
            .Where(x => !x.Name.Contains("Component"))
            .Where(x => !x.Parameters.Any(p => p.Type.TypeKind == TypeKind.Array))
            .Where(x => !x.Parameters.Any(p => !p.Type.IsAction() && !p.Type.IsFunc() && p.Type.TypeKind == TypeKind.Delegate))
            .Apply(Remove("global::System.Predicate"))
            .Apply(Remove("BoxShadowStyle"))
            .Apply(Remove("TextStyle"))
            .Apply(Remove("IDynamic"))
            .Apply(Remove("Image"))
            .Apply(Remove("SvgImage"))
            .Apply(Remove("Stream"))
            .Apply(Remove("Size"))
            .Apply(Remove("IDynamicElement"))
            .Where(x => !(x.Parameters.Skip(1).FirstOrDefault()?.Type?.Name?.Contains("IContainer") ?? false))
            .Where(x => !x.Parameters.Any(p => p.Type.SpecialType == SpecialType.System_Array))
            .Where(x => !x.Parameters.Any(p => p.GetAttributes().Any()));

        Func<IEnumerable<IMethodSymbol>, IEnumerable<IMethodSymbol>> Remove(string phrase)
        {
            return x => x.Where(x => !x.Parameters.Any(p => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Contains(phrase)));
        }
    }
    
    // IEnumearable apply
    public static IEnumerable<IMethodSymbol> Apply(this IEnumerable<IMethodSymbol> methodSymbols, Func<IEnumerable<IMethodSymbol>, IEnumerable<IMethodSymbol>> filter)
    {
        return filter(methodSymbols);
    }
    
    public static bool InheritsFromOrEquals(this ITypeSymbol type, ITypeSymbol baseType)
    {
        // Check for equality first (Roslyn requires SymbolEqualityComparer)
        if (SymbolEqualityComparer.Default.Equals(type, baseType))
        {
            return true;
        }

        // Walk up the inheritance chain
        var current = type.BaseType;
        while (current != null)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
            {
                return true;
            }
            current = current.BaseType;
        }

        // Note: If you need to check Interfaces, you must also check type.AllInterfaces
        return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, baseType));
    }
    
    public static bool IsExtensionFor(this IMethodSymbol method, ITypeSymbol targetType)
    {
        // 1. Basic check: Is it actually an extension method?
        if (!method.IsExtensionMethod || method.Parameters.IsEmpty)
        {
            return false;
        }

        // 2. Get the type of the 'this' parameter (always the first one)
        var thisParamType = method.Parameters[0].Type;

        // 3. Handle Generic Parameters (e.g., public static void Foo<T>(this T item) where T : MyClass)
        if (thisParamType is ITypeParameterSymbol typeParam)
        {
            // If the generic is unconstrained (extends Object), it technically extends *everything*.
            // If you ONLY want methods specifically constrained to your class, return false here if constraints are empty.
            if (!typeParam.ConstraintTypes.Any())
            {
                // Returns true because "this T" applies to "MyClass", 
                // but change to false if you strictly want "where T : MyClass"
                return true; 
            }

            // Check if any of the constraints imply the target type
            return typeParam.ConstraintTypes.Any(constraint => targetType.InheritsFromOrEquals(constraint));
        }

        // 4. Handle Direct Types (e.g., public static void Foo(this MyClass item))
        // We check if our TargetType inherits from (or is equal to) the parameter type.
        // Note: Logic is reversed here. If method extends "BaseClass", "MyClass" is valid.
        return targetType.InheritsFromOrEquals(thisParamType);
    }
}