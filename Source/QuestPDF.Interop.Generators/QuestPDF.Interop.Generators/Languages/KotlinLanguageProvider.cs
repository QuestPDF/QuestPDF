using System.Collections.Generic;
using System.Linq;
using QuestPDF.Interop.Generators.Models;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Language provider for Kotlin code generation using JNA (Java Native Access).
/// </summary>
public class KotlinLanguageProvider : ILanguageProvider
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

    public string GetTargetType(InteropTypeModel type)
    {
        return type.Kind switch
        {
            InteropTypeKind.Void => "Unit",
            InteropTypeKind.Boolean => "Boolean",
            InteropTypeKind.Integer => GetKotlinIntegerType(type),
            InteropTypeKind.Float => GetKotlinFloatType(type),
            InteropTypeKind.String => "String",
            InteropTypeKind.Enum => ConvertTypeName(type.ShortName),
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
            return $"{ConvertTypeName(parameter.Type.ShortName)}.{parameter.DefaultEnumMemberName}";

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

    public object BuildClassTemplateModel(InteropClassModel classModel, string customDefinitions, string customInit, string customClass)
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
            InheritFrom = classModel.InheritFrom,
            Methods = classModel.Methods.Select(BuildMethodTemplateModel).ToList(),
            CallbackInterfaces = callbackInterfaces,
            CallbackTypedefs = classModel.CallbackTypedefs,
            Headers = classModel.CHeaderSignatures,
            CustomDefinitions = customDefinitions,
            CustomInit = customInit,
            CustomClass = customClass
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
        var nativeArgs = new List<string>();

        if (!method.IsStaticMethod)
            nativeArgs.Add("pointer");

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
        var uniqueId = method.NativeEntryPoint.ExtractNativeMethodHash();

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
            IsStaticMethod = method.IsStaticMethod,
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

        // Build parameters: first is Pointer target for instance methods
        var parameters = new List<string>();

        if (!method.IsStaticMethod)
            parameters.Add("target: Pointer");

        parameters.AddRange(method.Parameters.Select(p =>
        {
            var jnaType = GetJnaType(p.Type);
            return $"{p.OriginalName}: {jnaType}";
        }));

        return $"fun {methodName}({string.Join(", ", parameters)}): {returnType}";
    }
}
