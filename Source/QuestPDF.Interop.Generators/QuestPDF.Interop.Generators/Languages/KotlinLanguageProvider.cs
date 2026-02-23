using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Language provider for Kotlin code generation using JNA (Java Native Access).
/// </summary>
internal class KotlinLanguageProvider : ILanguageProvider
{
    // Store current class name for TypeParameter resolution
    private string _currentClassName;

    /// <summary>
    /// Maps C# type names to Kotlin-safe equivalents.
    /// This handles reserved keywords and naming conflicts.
    /// </summary>
    private static readonly Dictionary<string, string> TypeNameMappings = new()
    {
        // "Unit" is a reserved keyword in Kotlin (equivalent to void)
        ["Unit"] = "LengthUnit"
    };

    /// <summary>
    /// Converts a C# type name to a Kotlin-safe type name.
    /// </summary>
    public static string ConvertTypeName(string csharpTypeName)
    {
        return TypeNameMappings.TryGetValue(csharpTypeName, out var kotlinName)
            ? kotlinName
            : csharpTypeName;
    }

    public string ConvertName(string csharpName, NameContext context)
    {
        return context switch
        {
            NameContext.Method => csharpName.ToCamelCase(),
            NameContext.Parameter => csharpName.ToCamelCase(),
            NameContext.EnumValue => csharpName, // UpperCamelCase (PascalCase)
            NameContext.Property => csharpName.ToCamelCase(),
            NameContext.Constant => csharpName.ToSnakeCase().ToUpperInvariant(),
            NameContext.Class => csharpName, // Keep PascalCase
            _ => csharpName
        };
    }

    public string GetTargetType(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Void => "Unit",
            InteropTypeKind.Boolean => "Boolean",
            InteropTypeKind.Integer => GetKotlinIntegerType(type),
            InteropTypeKind.Float => GetKotlinFloatType(type),
            InteropTypeKind.String => "String",
            InteropTypeKind.Enum => ConvertTypeName(type.Name),
            InteropTypeKind.Class => type.Name,
            InteropTypeKind.Interface => type.Name.TrimStart('I'),
            InteropTypeKind.TypeParameter => _currentClassName ?? "Any",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Action => FormatFunctionType((INamedTypeSymbol)type, isFunc: false),
            InteropTypeKind.Func => FormatFunctionType((INamedTypeSymbol)type, isFunc: true),
            InteropTypeKind.Unknown => "Any",
            _ => "Any"
        };
    }

    /// <summary>
    /// Gets the JNA-compatible type for native library interface declarations.
    /// </summary>
    public string GetJnaType(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Void => "Unit",
            InteropTypeKind.Boolean => "Byte", // JNA uses byte for C bool
            InteropTypeKind.Integer => GetJnaIntegerType(type),
            InteropTypeKind.Float => GetJnaFloatType(type),
            InteropTypeKind.String => "String",
            InteropTypeKind.Enum => "Int",
            InteropTypeKind.Class => "Pointer",
            InteropTypeKind.Interface => "Pointer",
            InteropTypeKind.TypeParameter => "Pointer",
            InteropTypeKind.Color => "Int", // ARGB packed as int (use Int to avoid Kotlin inline class name mangling)
            InteropTypeKind.Action => "Callback", // Callback pointer
            InteropTypeKind.Func => "Callback", // Callback pointer
            InteropTypeKind.Unknown => "Pointer",
            _ => "Pointer"
        };
    }

    private string GetKotlinIntegerType(ITypeSymbol type)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (typeName.Contains("Int64") || typeName.Contains("UInt64"))
            return "Long";
        if (typeName.Contains("Int16") || typeName.Contains("UInt16"))
            return "Short";
        if (typeName.Contains("Byte") || typeName.Contains("SByte"))
            return "Byte";
        return "Int";
    }

    private string GetJnaIntegerType(ITypeSymbol type)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (typeName.Contains("Int64") || typeName.Contains("UInt64"))
            return "Long";
        if (typeName.Contains("Int16") || typeName.Contains("UInt16"))
            return "Short";
        if (typeName.Contains("Byte") || typeName.Contains("SByte"))
            return "Byte";
        return "Int";
    }

    private string GetKotlinFloatType(ITypeSymbol type)
    {
        return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Contains("Double") ? "Double" : "Float";
    }

    private string GetJnaFloatType(ITypeSymbol type)
    {
        return type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Contains("Double") ? "Double" : "Float";
    }

    private string FormatFunctionType(INamedTypeSymbol type, bool isFunc)
    {
        if (type.TypeArguments.Length == 0)
            return isFunc ? "() -> Any" : "() -> Unit";

        if (isFunc)
        {
            var args = type.TypeArguments.Take(type.TypeArguments.Length - 1)
                .Select(t => GetTargetTypeForCallback(t));
            var returnType = GetTargetTypeForCallback(type.TypeArguments.Last());
            return $"({string.Join(", ", args)}) -> {returnType}";
        }
        else
        {
            var args = type.TypeArguments.Select(t => GetTargetTypeForCallback(t));
            return $"({string.Join(", ", args)}) -> Unit";
        }
    }

    private string GetTargetTypeForCallback(ITypeSymbol type)
    {
        if (type.GetInteropTypeKind() == InteropTypeKind.Interface)
            return type.Name.TrimStart('I');
        return type.Name;
    }

    public string FormatDefaultValue(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        if (parameter.ExplicitDefaultValue == null)
            return "null";

        var kind = parameter.Type.GetInteropTypeKind();

        if (kind == InteropTypeKind.Enum && parameter.GetDefaultEnumMemberName() != null)
            return $"{ConvertTypeName(parameter.Type.Name)}.{parameter.GetDefaultEnumMemberName()}";

        if (parameter.ExplicitDefaultValue is bool boolValue)
            return boolValue ? "true" : "false";

        if (parameter.ExplicitDefaultValue is string stringValue)
            return $"\"{stringValue}\"";

        // For floating point defaults, add 'f' suffix for Float type
        if (kind == InteropTypeKind.Float)
        {
            var floatType = GetKotlinFloatType(parameter.Type);
            if (floatType == "Float")
                return $"{parameter.ExplicitDefaultValue}f";
        }

        return parameter.ExplicitDefaultValue?.ToString() ?? "null";
    }

    public string GetInteropValue(IParameterSymbol parameter, string variableName)
    {
        var kind = parameter.Type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Enum => $"{variableName}.value",
            InteropTypeKind.String => variableName,
            InteropTypeKind.Boolean => $"(if ({variableName}) 1.toByte() else 0.toByte())",
            InteropTypeKind.Color => $"{variableName}.hex.toInt()", // Convert UInt to Int to avoid inline class name mangling
            InteropTypeKind.Action => $"{variableName}Callback",
            InteropTypeKind.Func => $"{variableName}Callback",
            InteropTypeKind.Class => $"{variableName}.pointer",
            InteropTypeKind.Interface => $"{variableName}.pointer",
            _ => variableName
        };
    }

    public object BuildClassTemplateModel(
        INamedTypeSymbol targetType,
        IReadOnlyList<IMethodSymbol> methods,
        Dictionary<IMethodSymbol, OverloadInfo> overloads,
        string inheritFrom,
        string customDefinitions, string customInit, string customClass)
    {
        var className = targetType.GetGeneratedClassName();
        _currentClassName = className;

        // Collect all unique callback interfaces needed
        var callbackInterfaces = methods
            .SelectMany(m => m.GetCallbackParameters())
            .Select(c => new
            {
                InterfaceName = $"{c.GetCallbackArgumentTypeName()}Callback",
                ArgumentTypeName = c.GetCallbackArgumentTypeName()
            })
            .DistinctBy(c => c.InterfaceName)
            .ToList();

        var callbackTypedefs = methods
            .GetCallbackTypedefs()
            .Select(t => t.TypedefDefinition)
            .Distinct()
            .ToList();

        var cHeaders = methods
            .Select(m => m.GetCHeaderDefinition(className))
            .ToList();

        return new
        {
            ClassName = className,
            InheritFrom = inheritFrom,
            Methods = methods.Select(m => BuildMethodTemplateModel(m, targetType, overloads)).ToList(),
            CallbackInterfaces = callbackInterfaces,
            CallbackTypedefs = callbackTypedefs,
            Headers = cHeaders,
            CustomDefinitions = customDefinitions,
            CustomInit = customInit,
            CustomClass = customClass
        };
    }

    private object BuildMethodTemplateModel(IMethodSymbol method, INamedTypeSymbol targetType, Dictionary<IMethodSymbol, OverloadInfo> overloads)
    {
        var isStaticMethod = method.IsStatic && !method.IsExtensionMethod;
        var nonThisParams = method.GetNonThisParameters();
        var overloadInfo = overloads[method];
        var className = targetType.GetGeneratedClassName();

        // Build Kotlin method parameter string
        var kotlinParams = nonThisParams.Select(p =>
        {
            var name = ConvertName(p.Name, NameContext.Parameter);
            var type = GetTargetType(p.Type);
            var defaultVal = p.HasExplicitDefaultValue ? FormatDefaultValue(p) : null;
            return new { Name = name, Type = type, OriginalParam = p, DefaultValue = defaultVal };
        }).ToList();

        var kotlinParametersStr = string.Join(", ", kotlinParams.Select(p =>
            p.DefaultValue != null ? $"{p.Name}: {p.Type} = {p.DefaultValue}" : $"{p.Name}: {p.Type}"));

        // Build native call arguments
        var nativeArgs = new List<string>();

        if (!isStaticMethod)
            nativeArgs.Add("pointer");

        nativeArgs.AddRange(nonThisParams.Select(p =>
            GetInteropValue(p, ConvertName(p.Name, NameContext.Parameter))));
        var nativeCallArgsStr = string.Join(", ", nativeArgs);

        // Build JNA interface method signature
        var jnaSignature = BuildJnaSignature(method, targetType);

        // Determine return type
        var returnTypeKind = method.ReturnType.GetInteropTypeKind();
        var kotlinReturnType = GetReturnTypeName(method);
        var jnaReturnType = returnTypeKind == InteropTypeKind.Void
            ? "Unit"
            : GetJnaType(method.ReturnType);
        var returnClassName = kotlinReturnType != "Unit" ? kotlinReturnType : null;

        // Extract unique ID from native entry point
        var nativeEntryPoint = method.GetNativeMethodName(className);
        var uniqueId = nativeEntryPoint.ExtractNativeMethodHash();

        // Use disambiguated name for overloads
        var methodName = overloadInfo.IsOverload
            ? ConvertName(overloadInfo.DisambiguatedName, NameContext.Method)
            : ConvertName(method.Name, NameContext.Method);

        return new
        {
            KotlinMethodName = methodName,
            NativeMethodName = nativeEntryPoint,
            KotlinParameters = kotlinParametersStr,
            KotlinReturnType = kotlinReturnType,
            JnaReturnType = jnaReturnType,
            JnaSignature = jnaSignature,
            ReturnClassName = returnClassName,
            NativeCallArgs = nativeCallArgsStr,
            UniqueId = uniqueId,
            DeprecationMessage = method.TryGetDeprecationMessage(),
            IsStaticMethod = isStaticMethod,
            Callbacks = method.GetCallbackParameters().Select(c => new
            {
                ParameterName = ConvertName(c.Name, NameContext.Parameter),
                ArgumentTypeName = c.GetCallbackArgumentTypeName(),
                CallbackInterfaceName = $"{c.GetCallbackArgumentTypeName()}Callback"
            }).ToList(),
            IsOverload = overloadInfo.IsOverload,
            OriginalName = ConvertName(method.Name, NameContext.Method),
            Parameters = kotlinParams.Select(p =>
            {
                var kind = p.OriginalParam.Type.GetInteropTypeKind();
                return new
                {
                    p.Name,
                    p.Type,
                    JnaType = GetJnaType(p.OriginalParam.Type),
                    IsCallback = kind is InteropTypeKind.Action or InteropTypeKind.Func,
                    IsEnum = kind == InteropTypeKind.Enum,
                    IsString = kind == InteropTypeKind.String,
                    IsBoolean = kind == InteropTypeKind.Boolean,
                    IsColor = kind == InteropTypeKind.Color,
                    IsObject = kind is InteropTypeKind.Class or InteropTypeKind.Interface
                };
            }).ToList(),
            ReturnsPointer = returnTypeKind is InteropTypeKind.Class or InteropTypeKind.Interface or InteropTypeKind.TypeParameter,
            HasCallbacks = method.GetCallbackParameters().Any()
        };
    }

    private string GetReturnTypeName(IMethodSymbol method)
    {
        var kind = method.ReturnType.GetInteropTypeKind();

        if (kind == InteropTypeKind.Void)
            return "Unit";

        if (kind == InteropTypeKind.TypeParameter)
            return _currentClassName;

        return GetTargetType(method.ReturnType);
    }

    private string BuildJnaSignature(IMethodSymbol method, INamedTypeSymbol targetType)
    {
        var returnType = GetJnaType(method.ReturnType);
        var className = targetType.GetGeneratedClassName();
        var methodName = method.GetNativeMethodName(className);
        var isStaticMethod = method.IsStatic && !method.IsExtensionMethod;

        // Build parameters: first is Pointer target for instance methods
        var parameters = new List<string>();

        if (!isStaticMethod)
            parameters.Add("target: Pointer");

        parameters.AddRange(method.GetNonThisParameters().Select(p =>
        {
            var jnaType = GetJnaType(p.Type);
            return $"{p.Name}: {jnaType}";
        }));

        return $"fun {methodName}({string.Join(", ", parameters)}): {returnType}";
    }
}
