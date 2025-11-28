using System.Collections.Generic;
using System.Linq;
using QuestPDF.Interop.Generators.Models;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Language provider for Kotlin code generation using JNA (Java Native Access).
/// </summary>
public class KotlinLanguageProvider : ILanguageProvider
{
    public string LanguageName => "Kotlin";
    public string FileExtension => ".kt";
    public string MainTemplateName => "Kotlin.Main";
    public string ObjectTemplateName => "Kotlin.Object";
    public string EnumTemplateName => "Kotlin.Enum";
    public string ColorsTemplateName => "Kotlin.Colors";

    // Store current class name for TypeParameter resolution
    private string _currentClassName;

    public string ConvertName(string csharpName, NameContext context)
    {
        return context switch
        {
            NameContext.Method => ToCamelCase(csharpName),
            NameContext.Parameter => ToCamelCase(csharpName),
            NameContext.EnumValue => csharpName.ToSnakeCase().ToUpperInvariant(), // SCREAMING_SNAKE_CASE
            NameContext.Property => ToCamelCase(csharpName),
            NameContext.Constant => csharpName.ToSnakeCase().ToUpperInvariant(),
            NameContext.Class => csharpName, // Keep PascalCase
            _ => csharpName
        };
    }

    private static string ToCamelCase(string pascalCase)
    {
        if (string.IsNullOrEmpty(pascalCase))
            return pascalCase;

        return char.ToLowerInvariant(pascalCase[0]) + pascalCase.Substring(1);
    }

    public string GetTargetType(InteropTypeModel type)
    {
        return type.Kind switch
        {
            InteropTypeKind.Void => "Unit",
            InteropTypeKind.Boolean => "Boolean",
            InteropTypeKind.Integer => GetKotlinIntegerType(type),
            InteropTypeKind.Float => GetKotlinFloatType(type),
            InteropTypeKind.String => "String",
            InteropTypeKind.Enum => type.ShortName,
            InteropTypeKind.Class => type.ShortName,
            InteropTypeKind.Interface => type.ShortName.TrimStart('I'),
            InteropTypeKind.TypeParameter => _currentClassName ?? "Any",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Action => FormatFunctionType(type, isFunc: false),
            InteropTypeKind.Func => FormatFunctionType(type, isFunc: true),
            InteropTypeKind.Unknown => "Any",
            _ => "Any"
        };
    }

    /// <summary>
    /// Gets the JNA-compatible type for native library interface declarations.
    /// </summary>
    public string GetJnaType(InteropTypeModel type)
    {
        return type.Kind switch
        {
            InteropTypeKind.Void => "Unit",
            InteropTypeKind.Boolean => "Byte", // JNA uses byte for C bool
            InteropTypeKind.Integer => GetJnaIntegerType(type),
            InteropTypeKind.Float => GetJnaFloatType(type),
            InteropTypeKind.String => "WString", // UTF-16 for .NET compatibility
            InteropTypeKind.Enum => "Int",
            InteropTypeKind.Class => "Pointer",
            InteropTypeKind.Interface => "Pointer",
            InteropTypeKind.TypeParameter => "Pointer",
            InteropTypeKind.Color => "UInt", // ARGB packed as int
            InteropTypeKind.Action => "Callback", // Callback pointer
            InteropTypeKind.Func => "Callback", // Callback pointer
            InteropTypeKind.Unknown => "Pointer",
            _ => "Pointer"
        };
    }

    private string GetKotlinIntegerType(InteropTypeModel type)
    {
        var typeName = type.OriginalTypeName ?? "";
        if (typeName.Contains("Int64") || typeName.Contains("UInt64"))
            return "Long";
        if (typeName.Contains("Int16") || typeName.Contains("UInt16"))
            return "Short";
        if (typeName.Contains("Byte") || typeName.Contains("SByte"))
            return "Byte";
        return "Int";
    }

    private string GetJnaIntegerType(InteropTypeModel type)
    {
        var typeName = type.OriginalTypeName ?? "";
        if (typeName.Contains("Int64") || typeName.Contains("UInt64"))
            return "Long";
        if (typeName.Contains("Int16") || typeName.Contains("UInt16"))
            return "Short";
        if (typeName.Contains("Byte") || typeName.Contains("SByte"))
            return "Byte";
        return "Int";
    }

    private string GetKotlinFloatType(InteropTypeModel type)
    {
        return type.OriginalTypeName?.Contains("Double") == true ? "Double" : "Float";
    }

    private string GetJnaFloatType(InteropTypeModel type)
    {
        return type.OriginalTypeName?.Contains("Double") == true ? "Double" : "Float";
    }

    private string FormatFunctionType(InteropTypeModel type, bool isFunc)
    {
        if (type.TypeArguments == null || type.TypeArguments.Length == 0)
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

    private string GetTargetTypeForCallback(InteropTypeModel type)
    {
        if (type.Kind == InteropTypeKind.Interface)
            return type.ShortName.TrimStart('I');
        return type.ShortName;
    }

    public string FormatDefaultValue(InteropParameterModel parameter)
    {
        if (!parameter.HasDefaultValue)
            return null;

        if (parameter.DefaultValue == null)
            return "null";

        if (parameter.Type.Kind == InteropTypeKind.Enum && parameter.DefaultEnumMemberName != null)
            return $"{parameter.Type.ShortName}.{parameter.DefaultEnumMemberName.ToSnakeCase().ToUpperInvariant()}";

        if (parameter.DefaultValue is bool boolValue)
            return boolValue ? "true" : "false";

        if (parameter.DefaultValue is string stringValue)
            return $"\"{stringValue}\"";

        // For floating point defaults, add 'f' suffix for Float type
        if (parameter.Type.Kind == InteropTypeKind.Float)
        {
            var floatType = GetKotlinFloatType(parameter.Type);
            if (floatType == "Float")
                return $"{parameter.DefaultValue}f";
        }

        return parameter.DefaultValue?.ToString() ?? "null";
    }

    public string GetInteropValue(InteropParameterModel parameter, string variableName)
    {
        return parameter.Type.Kind switch
        {
            InteropTypeKind.Enum => $"{variableName}.value",
            InteropTypeKind.String => $"WString({variableName})",
            InteropTypeKind.Boolean => $"(if ({variableName}) 1.toByte() else 0.toByte())",
            InteropTypeKind.Color => $"{variableName}.hex",
            InteropTypeKind.Action => $"{variableName}Callback",
            InteropTypeKind.Func => $"{variableName}Callback",
            InteropTypeKind.Class => $"{variableName}.pointer",
            InteropTypeKind.Interface => $"{variableName}.pointer",
            _ => variableName
        };
    }

    public object BuildClassTemplateModel(InteropClassModel classModel)
    {
        _currentClassName = classModel.GeneratedClassName;

        // Collect all unique callback interfaces needed
        var callbackInterfaces = classModel.Methods
            .SelectMany(m => m.Callbacks)
            .Select(c => new
            {
                InterfaceName = $"{c.ArgumentTypeName}Callback",
                ArgumentTypeName = c.ArgumentTypeName
            })
            .DistinctBy(c => c.InterfaceName)
            .ToList();

        return new
        {
            ClassName = classModel.GeneratedClassName,
            Methods = classModel.Methods.Select(BuildMethodTemplateModel).ToList(),
            CallbackInterfaces = callbackInterfaces,
            CallbackTypedefs = classModel.CallbackTypedefs,
            Headers = classModel.CHeaderSignatures
        };
    }

    private object BuildMethodTemplateModel(InteropMethodModel method)
    {
        // Build Kotlin method parameter string
        var kotlinParams = method.Parameters.Select(p =>
        {
            var name = ConvertName(p.OriginalName, NameContext.Parameter);
            var type = GetTargetType(p.Type);
            var defaultVal = p.HasDefaultValue ? FormatDefaultValue(p) : null;
            return new { Name = name, Type = type, OriginalType = p.Type, DefaultValue = defaultVal };
        }).ToList();

        var kotlinParametersStr = string.Join(", ", kotlinParams.Select(p =>
            p.DefaultValue != null ? $"{p.Name}: {p.Type} = {p.DefaultValue}" : $"{p.Name}: {p.Type}"));

        // Build native call arguments
        var nativeArgs = new List<string> { "pointer" };
        nativeArgs.AddRange(method.Parameters.Select(p =>
            GetInteropValue(p, ConvertName(p.OriginalName, NameContext.Parameter))));
        var nativeCallArgsStr = string.Join(", ", nativeArgs);

        // Build JNA interface method signature
        var jnaSignature = BuildJnaSignature(method);

        // Determine return type
        var kotlinReturnType = GetReturnTypeName(method);
        var jnaReturnType = method.ReturnType.Kind == InteropTypeKind.Void
            ? "Unit"
            : GetJnaType(method.ReturnType);
        var returnClassName = kotlinReturnType != "Unit" ? kotlinReturnType : null;

        // Extract unique ID from native entry point
        var uniqueId = ExtractUniqueId(method.NativeEntryPoint);

        // Use disambiguated name for overloads
        var methodName = method.IsOverload
            ? ConvertName(method.DisambiguatedName, NameContext.Method)
            : ConvertName(method.OriginalName, NameContext.Method);

        return new
        {
            KotlinMethodName = methodName,
            NativeMethodName = method.NativeEntryPoint,
            KotlinParameters = kotlinParametersStr,
            KotlinReturnType = kotlinReturnType,
            JnaReturnType = jnaReturnType,
            JnaSignature = jnaSignature,
            ReturnClassName = returnClassName,
            NativeCallArgs = nativeCallArgsStr,
            UniqueId = uniqueId,
            DeprecationMessage = method.DeprecationMessage,
            Callbacks = method.Callbacks.Select(c => new
            {
                ParameterName = ConvertName(c.ParameterName, NameContext.Parameter),
                ArgumentTypeName = c.ArgumentTypeName,
                CallbackInterfaceName = $"{c.ArgumentTypeName}Callback"
            }).ToList(),
            IsOverload = method.IsOverload,
            OriginalName = ConvertName(method.OriginalName, NameContext.Method),
            Parameters = kotlinParams.Select(p => new
            {
                p.Name,
                p.Type,
                JnaType = GetJnaType(p.OriginalType),
                IsCallback = p.OriginalType.Kind is InteropTypeKind.Action or InteropTypeKind.Func,
                IsEnum = p.OriginalType.Kind == InteropTypeKind.Enum,
                IsString = p.OriginalType.Kind == InteropTypeKind.String,
                IsBoolean = p.OriginalType.Kind == InteropTypeKind.Boolean,
                IsColor = p.OriginalType.Kind == InteropTypeKind.Color,
                IsObject = p.OriginalType.Kind is InteropTypeKind.Class or InteropTypeKind.Interface
            }).ToList(),
            ReturnsPointer = method.ReturnType.Kind is InteropTypeKind.Class or InteropTypeKind.Interface or InteropTypeKind.TypeParameter,
            HasCallbacks = method.Callbacks.Any()
        };
    }

    private static string ExtractUniqueId(string nativeEntryPoint)
    {
        var lastUnderscore = nativeEntryPoint.LastIndexOf("__");
        if (lastUnderscore >= 0 && lastUnderscore < nativeEntryPoint.Length - 2)
        {
            return nativeEntryPoint.Substring(lastUnderscore + 2);
        }
        return nativeEntryPoint.GetHashCode().ToString("x8");
    }

    private string GetReturnTypeName(InteropMethodModel method)
    {
        if (method.ReturnType.Kind == InteropTypeKind.Void)
            return "Unit";

        if (method.ReturnType.Kind == InteropTypeKind.TypeParameter)
            return _currentClassName;

        return GetTargetType(method.ReturnType);
    }

    private string BuildJnaSignature(InteropMethodModel method)
    {
        var returnType = GetJnaType(method.ReturnType);
        var methodName = method.NativeEntryPoint;

        // Build parameters: first is always Pointer target
        var parameters = new List<string> { "target: Pointer" };
        parameters.AddRange(method.Parameters.Select(p =>
        {
            var jnaType = GetJnaType(p.Type);
            return $"{p.OriginalName}: {jnaType}";
        }));

        return $"fun {methodName}({string.Join(", ", parameters)}): {returnType}";
    }

    public object BuildEnumTemplateModel(object enums)
    {
        return new { Enums = enums };
    }

    public object BuildColorsTemplateModel(object basicColors, object colorGroups)
    {
        return new { BasicColors = basicColors, ColorGroups = colorGroups };
    }
}
