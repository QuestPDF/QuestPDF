using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

/// <summary>
/// Categorizes types for code generation purposes.
/// </summary>
internal enum InteropTypeKind
{
    Void,
    Boolean,
    Int,
    Float,
    String,
    Enum,
    Class,
    Interface,
    Action,
    Func,
    TypeParameter,
    Color,
    ByteArray,
    Size,
    ImageSize,
    Unknown
}

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

    /// <summary>
    /// Extracts the unique hash suffix from a native entry point name.
    /// Native entry points end with a hash like "__a1b2c3d4".
    /// </summary>
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
                // Use typedef name for callbacks instead of inline function pointer
                if (x.Type.IsAction() || x.Type.IsFunc())
                    return $"{x.Type.GetCallbackTypedefName(methodSymbol)} {x.Name}";

                return $"{GetNativeParameterType(x.Type)} {x.Name}";
            });

        if (!methodSymbol.IsExtensionMethod && !methodSymbol.IsStatic)
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
            .ExcludeOldObsoleteMethods()
            .Where(x => !x.Parameters.Any(p => p.Type.TypeKind == TypeKind.Array))
            .Where(x => !x.Name.Contains("Component"))
            .Where(x => !x.Parameters.Any(p => !p.Type.IsAction() && !p.Type.IsFunc() && p.Type.TypeKind == TypeKind.Delegate))
            .Apply(Remove("global::System.Predicate"))
            .Apply(Remove("BoxShadowStyle"))
            .Apply(Remove("TextStyle"))
            .Apply(Remove("IDynamic"))
            .Apply(Remove("Stream"))
            .Apply(Remove("IDynamicElement"))
            .Where(x => x.Name != "Dispose")
            .Where(x => !(x.Parameters.Skip(1).FirstOrDefault()?.Type?.Name?.Contains("IContainer") ?? false))
            .Where(x => !x.Parameters.Any(p => p.GetAttributes().Any()));

        Func<IEnumerable<IMethodSymbol>, IEnumerable<IMethodSymbol>> Remove(string phrase)
        {
            return x => x.Where(x => !x.Parameters.Any(p => p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Contains(phrase)));
        }
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

    /// <summary>
    /// Applies a filter function to the method collection.
    /// Useful for building fluent filter chains.
    /// </summary>
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

    // =========================================================================
    // Type classification and overload handling
    // =========================================================================

    public static InteropTypeKind GetInteropTypeKind(this ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_Void)
            return InteropTypeKind.Void;

        if (type.SpecialType == SpecialType.System_Boolean)
            return InteropTypeKind.Boolean;

        if (type.SpecialType == SpecialType.System_String)
            return InteropTypeKind.String;

        if (type.TypeKind == TypeKind.Enum)
            return InteropTypeKind.Enum;

        if (type.TypeKind == TypeKind.TypeParameter)
            return InteropTypeKind.TypeParameter;

        if (type.IsAction())
            return InteropTypeKind.Action;

        if (type.IsFunc())
            return InteropTypeKind.Func;

        if (type.TypeKind == TypeKind.Interface)
            return InteropTypeKind.Interface;

        if (type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Contains("QuestPDF.Infrastructure.Color"))
            return InteropTypeKind.Color;

        if (type.TypeKind == TypeKind.Class)
            return InteropTypeKind.Class;

        if (type.ToDisplayString() == "byte[]")
            return InteropTypeKind.ByteArray;

        if (type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Contains("QuestPDF.Infrastructure.ImageSize"))
            return InteropTypeKind.ImageSize;

        if (type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Contains("QuestPDF.Infrastructure.Size"))
            return InteropTypeKind.Size;

        if (type.SpecialType is SpecialType.System_Int32 or SpecialType.System_UInt32
            or SpecialType.System_Int64 or SpecialType.System_UInt64
            or SpecialType.System_Int16 or SpecialType.System_UInt16
            or SpecialType.System_Byte or SpecialType.System_SByte)
            return InteropTypeKind.Int;

        if (type.SpecialType is SpecialType.System_Single or SpecialType.System_Double)
            return InteropTypeKind.Float;

        return InteropTypeKind.Unknown;
    }

    public static string GetGeneratedClassName(this INamedTypeSymbol type)
    {
        return type.TypeKind == TypeKind.Interface
            ? type.Name.TrimStart('I')
            : type.Name;
    }

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
        if (parameter.Type is not INamedTypeSymbol delegateType)
            return null;

        var argumentType = delegateType.TypeArguments.FirstOrDefault();
        if (argumentType == null)
            return null;

        return argumentType.TypeKind == TypeKind.Interface
            ? argumentType.Name.TrimStart('I')
            : argumentType.Name;
    }

    public static Dictionary<IMethodSymbol, OverloadInfo> ComputeOverloads(this IReadOnlyList<IMethodSymbol> methods)
    {
        var result = new Dictionary<IMethodSymbol, OverloadInfo>(SymbolEqualityComparer.Default);
        var methodGroups = methods.GroupBy(m => m.Name);

        foreach (var group in methodGroups)
        {
            var groupMethods = group.ToList();

            if (groupMethods.Count == 1)
            {
                result[groupMethods[0]] = new OverloadInfo(false, groupMethods[0].Name, string.Empty);
            }
            else
            {
                // Generate initial suffixes
                var entries = groupMethods.Select(m => (Method: m, Suffix: GenerateOverloadSuffix(m))).ToList();

                // Resolve collisions
                var suffixGroups = entries.GroupBy(x => x.Suffix);

                foreach (var suffixGroup in suffixGroups)
                {
                    var collisions = suffixGroup.ToList();
                    if (collisions.Count > 1)
                    {
                        for (int i = 0; i < collisions.Count; i++)
                        {
                            var suffix = $"{collisions[i].Suffix}_{i + 1}";
                            result[collisions[i].Method] = new OverloadInfo(true, collisions[i].Method.Name + suffix, suffix);
                        }
                    }
                    else
                    {
                        var suffix = collisions[0].Suffix;
                        result[collisions[0].Method] = new OverloadInfo(true, collisions[0].Method.Name + suffix, suffix);
                    }
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
        var type = parameter.Type;
        var kind = type.GetInteropTypeKind();

        if (kind is InteropTypeKind.Enum or InteropTypeKind.Class)
            return type.Name;
        
        if (kind is InteropTypeKind.Interface)
            return type.Name.TrimStart('I');

        return kind.ToString();
    }

    [GeneratedRegex(@"(?<year>20\d{2})\.(?<month>0?[1-9]|1[0-2])")]
    private static partial Regex GetVersionParser();
}
