using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Models;

/// <summary>
/// Builds language-agnostic interop models from Roslyn symbols.
/// </summary>
internal static class InteropModelBuilder
{
    /// <summary>
    /// Builds a complete class model from target type and its methods.
    /// </summary>
    public static InteropClassModel BuildClassModel(
        INamedTypeSymbol targetType,
        IEnumerable<IMethodSymbol> methods)
    {
        var methodsList = methods.ToList();
        var methodModels = methodsList.Select(m => BuildMethodModel(m, targetType)).ToList();

        // Preprocess: detect and mark overloaded methods
        PreprocessOverloads(methodModels);

        var callbackTypedefs = methodsList
            .GetCallbackTypedefs()
            .Select(t => t.TypedefDefinition)
            .Distinct()
            .ToList();

        var generatedClassName = targetType.TypeKind == TypeKind.Interface
            ? targetType.Name.TrimStart('I')
            : targetType.Name;

        var cHeaders = methodsList
            .Select(m => m.GetCHeaderDefinition(generatedClassName))
            .ToList();

        return new InteropClassModel
        {
            OriginalTypeName = targetType.Name,
            GeneratedClassName = generatedClassName,
            IsStaticClass = targetType.IsStatic,
            IsInterface = targetType.TypeKind == TypeKind.Interface,
            Methods = methodModels,
            CallbackTypedefs = callbackTypedefs,
            CHeaderSignatures = cHeaders
        };
    }

    /// <summary>
    /// Preprocesses method models to detect and mark overloaded methods.
    /// For overloaded methods, sets IsOverload=true and generates a disambiguated name.
    /// </summary>
    private static void PreprocessOverloads(List<InteropMethodModel> methods)
    {
        // Group methods by their original name
        var methodGroups = methods.GroupBy(m => m.OriginalName).ToList();

        foreach (var group in methodGroups)
        {
            var groupMethods = group.ToList();

            if (groupMethods.Count == 1)
            {
                // Single method - no overload
                var method = groupMethods[0];
                method.IsOverload = false;
                method.OverloadSuffix = string.Empty;
                method.DisambiguatedName = method.OriginalName;
            }
            else
            {
                // Multiple methods with same name - mark as overloads and generate suffixes
                foreach (var method in groupMethods)
                {
                    method.IsOverload = true;
                    method.OverloadSuffix = GenerateOverloadSuffix(method);
                    method.DisambiguatedName = method.OriginalName + method.OverloadSuffix;
                }

                // Check for suffix collisions and resolve them
                ResolveOverloadSuffixCollisions(groupMethods);
            }
        }
    }

    /// <summary>
    /// Generates a suffix based on parameter types for method disambiguation.
    /// </summary>
    private static string GenerateOverloadSuffix(InteropMethodModel method)
    {
        if (method.Parameters.Count == 0)
            return "_NoArgs";

        var parts = method.Parameters.Select(GetTypeShortName);
        return "_" + string.Join("_", parts);
    }

    /// <summary>
    /// Gets a short name for a type suitable for use in method name suffixes.
    /// </summary>
    private static string GetTypeShortName(InteropParameterModel parameter)
    {
        var type = parameter.Type;

        return type.Kind switch
        {
            InteropTypeKind.Void => "Void",
            InteropTypeKind.Boolean => "Bool",
            InteropTypeKind.Integer => "Int",
            InteropTypeKind.Float => "Float",
            InteropTypeKind.String => "String",
            InteropTypeKind.Enum => type.ShortName,
            InteropTypeKind.Class => type.ShortName,
            InteropTypeKind.Interface => type.ShortName.TrimStart('I'),
            InteropTypeKind.Action => "Action",
            InteropTypeKind.Func => "Func",
            InteropTypeKind.TypeParameter => "T",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.ByteArray => "Bytes",
            InteropTypeKind.Size => "Size",
            InteropTypeKind.ImageSize => "ImageSize",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Resolves suffix collisions by appending a numeric index if needed.
    /// </summary>
    private static void ResolveOverloadSuffixCollisions(List<InteropMethodModel> methods)
    {
        var suffixGroups = methods.GroupBy(m => m.OverloadSuffix).ToList();

        foreach (var suffixGroup in suffixGroups)
        {
            var collisions = suffixGroup.ToList();
            if (collisions.Count > 1)
            {
                // Collision detected - append index
                for (int i = 0; i < collisions.Count; i++)
                {
                    collisions[i].OverloadSuffix += $"_{i + 1}";
                    collisions[i].DisambiguatedName = collisions[i].OriginalName + collisions[i].OverloadSuffix;
                }
            }
        }
    }

    /// <summary>
    /// Builds a method model from a Roslyn method symbol.
    /// </summary>
    public static InteropMethodModel BuildMethodModel(IMethodSymbol method, INamedTypeSymbol targetType)
    {
        var generatedClassName = targetType.TypeKind == TypeKind.Interface
            ? targetType.Name.TrimStart('I')
            : targetType.Name;

        var parameters = method.Parameters
            .Skip(method.IsExtensionMethod ? 1 : 0)
            .Select(p => BuildParameterModel(p, method))
            .ToList();

        var callbacks = BuildCallbackModels(method).ToList();

        return new InteropMethodModel
        {
            OriginalName = method.Name,
            NativeEntryPoint = method.GetNativeMethodName(generatedClassName),
            ManagedMethodName = method.GetManagedMethodName(generatedClassName),
            IsStaticMethod = method.IsStatic && !method.IsExtensionMethod,
            IsExtensionMethod = method.IsExtensionMethod,
            DeprecationMessage = method.TryGetDeprecationMessage(),
            ReturnType = BuildTypeModel(method.ReturnType, method),
            Parameters = parameters,
            Callbacks = callbacks,
            CHeaderSignature = method.GetCHeaderDefinition(generatedClassName)
        };
    }

    /// <summary>
    /// Builds a parameter model from a Roslyn parameter symbol.
    /// </summary>
    public static InteropParameterModel BuildParameterModel(IParameterSymbol parameter, IMethodSymbol containingMethod)
    {
        string defaultEnumMemberName = null;

        if (parameter.HasExplicitDefaultValue && parameter.Type.TypeKind == TypeKind.Enum)
        {
            defaultEnumMemberName = parameter.Type
                .GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(x => x.HasConstantValue && x.ConstantValue.Equals(parameter.ExplicitDefaultValue))
                ?.Name;
        }

        return new InteropParameterModel
        {
            OriginalName = parameter.Name,
            Type = BuildTypeModel(parameter.Type, containingMethod),
            HasDefaultValue = parameter.HasExplicitDefaultValue,
            DefaultValue = parameter.HasExplicitDefaultValue ? parameter.ExplicitDefaultValue : null,
            DefaultEnumMemberName = defaultEnumMemberName
        };
    }

    /// <summary>
    /// Builds a type model from a Roslyn type symbol.
    /// </summary>
    public static InteropTypeModel BuildTypeModel(ITypeSymbol type, IMethodSymbol containingMethod = null)
    {
        var kind = DetermineTypeKind(type);
        InteropTypeModel[] typeArguments = null;
        string callbackTypedefName = null;
        string callbackTypedefDefinition = null;

        if ((type.IsAction() || type.IsFunc()) && type is INamedTypeSymbol delegateType)
        {
            typeArguments = delegateType.TypeArguments
                .Select(t => BuildTypeModel(t, containingMethod))
                .ToArray();

            if (containingMethod != null)
            {
                callbackTypedefName = type.GetCallbackTypedefName(containingMethod);
                callbackTypedefDefinition = type.GetCallbackTypedefDefinition(containingMethod);
            }
        }

        return new InteropTypeModel
        {
            OriginalTypeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            ShortName = type.Name,
            Kind = kind,
            NativeType = type.GetNativeParameterType(),
            InteropType = type.GetInteropMethodParameterType(),
            TypeArguments = typeArguments,
            CallbackTypedefName = callbackTypedefName,
            CallbackTypedefDefinition = callbackTypedefDefinition
        };
    }

    /// <summary>
    /// Builds callback models for all Action/Func parameters in a method.
    /// </summary>
    private static IEnumerable<InteropCallbackModel> BuildCallbackModels(IMethodSymbol method)
    {
        var parameters = method.Parameters.Skip(method.IsExtensionMethod ? 1 : 0);

        foreach (var param in parameters.Where(p => p.Type.IsAction() || p.Type.IsFunc()))
        {
            var delegateType = (INamedTypeSymbol)param.Type;
            var isFunc = param.Type.IsFunc();

            var argumentType = delegateType.TypeArguments.FirstOrDefault();
            InteropTypeModel returnType = null;

            if (isFunc && delegateType.TypeArguments.Length > 0)
            {
                returnType = BuildTypeModel(delegateType.TypeArguments.Last(), method);
            }

            var argumentTypeName = argumentType != null
                ? (argumentType.TypeKind == TypeKind.Interface
                    ? argumentType.Name.TrimStart('I')
                    : argumentType.Name)
                : null;

            // Build native signature (e.g., "void(void*)" for Action<IContainer>)
            var argTypes = isFunc
                ? delegateType.TypeArguments.Take(delegateType.TypeArguments.Length - 1)
                : delegateType.TypeArguments;
            var nativeArgs = string.Join(", ", argTypes.Select(t => t.GetNativeParameterType()));
            var nativeReturn = isFunc ? delegateType.TypeArguments.Last().GetNativeParameterType() : "void";
            var nativeSignature = $"{nativeReturn}({nativeArgs})";

            yield return new InteropCallbackModel
            {
                ParameterName = param.Name,
                ArgumentTypeName = argumentTypeName,
                ArgumentType = argumentType != null ? BuildTypeModel(argumentType, method) : null,
                ReturnType = returnType,
                HasReturnValue = isFunc,
                NativeSignature = nativeSignature
            };
        }
    }

    /// <summary>
    /// Determines the InteropTypeKind for a given type symbol.
    /// </summary>
    private static InteropTypeKind DetermineTypeKind(ITypeSymbol type)
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
        
        // Check for numeric types
        var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (IsIntegerType(typeName))
            return InteropTypeKind.Integer;

        if (IsFloatType(typeName))
            return InteropTypeKind.Float;

        return InteropTypeKind.Unknown;
    }

    private static bool IsIntegerType(string typeName)
    {
        return typeName is "int" or "System.Int32" or "global::System.Int32"
            or "uint" or "System.UInt32" or "global::System.UInt32"
            or "long" or "System.Int64" or "global::System.Int64"
            or "ulong" or "System.UInt64" or "global::System.UInt64"
            or "short" or "System.Int16" or "global::System.Int16"
            or "ushort" or "System.UInt16" or "global::System.UInt16"
            or "byte" or "System.Byte" or "global::System.Byte"
            or "sbyte" or "System.SByte" or "global::System.SByte";
    }

    private static bool IsFloatType(string typeName)
    {
        return typeName is "float" or "System.Single" or "global::System.Single"
            or "double" or "System.Double" or "global::System.Double";
    }
}
