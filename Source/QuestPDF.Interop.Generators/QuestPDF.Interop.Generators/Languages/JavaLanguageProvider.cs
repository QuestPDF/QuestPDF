using System.Collections.Generic;
using System.Linq;
using QuestPDF.Interop.Generators.Models;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Language provider for Java code generation using JNA (Java Native Access).
/// </summary>
public class JavaLanguageProvider : ILanguageProvider
{
    public string LanguageName => "Java";
    public string FileExtension => ".java";
    public string MainTemplateName => "Java.Main";
    public string ObjectTemplateName => "Java.Object";
    public string EnumTemplateName => "Java.Enum";
    public string ColorsTemplateName => "Java.Colors";

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
            InteropTypeKind.Void => "void",
            InteropTypeKind.Boolean => "boolean",
            InteropTypeKind.Integer => GetJavaIntegerType(type),
            InteropTypeKind.Float => GetJavaFloatType(type),
            InteropTypeKind.String => "String",
            InteropTypeKind.Enum => type.ShortName,
            InteropTypeKind.Class => type.ShortName,
            InteropTypeKind.Interface => type.ShortName.TrimStart('I'),
            InteropTypeKind.TypeParameter => _currentClassName ?? "Object",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Action => FormatFunctionalInterface(type, isFunc: false),
            InteropTypeKind.Func => FormatFunctionalInterface(type, isFunc: true),
            InteropTypeKind.Unknown => "Object",
            _ => "Object"
        };
    }

    /// <summary>
    /// Gets the JNA-compatible type for native library interface declarations.
    /// </summary>
    public string GetJnaType(InteropTypeModel type)
    {
        return type.Kind switch
        {
            InteropTypeKind.Void => "void",
            InteropTypeKind.Boolean => "byte", // JNA uses byte for C bool
            InteropTypeKind.Integer => GetJnaIntegerType(type),
            InteropTypeKind.Float => GetJnaFloatType(type),
            InteropTypeKind.String => "WString", // UTF-16 for .NET compatibility
            InteropTypeKind.Enum => "int",
            InteropTypeKind.Class => "Pointer",
            InteropTypeKind.Interface => "Pointer",
            InteropTypeKind.TypeParameter => "Pointer",
            InteropTypeKind.Color => "int", // ARGB packed as int
            InteropTypeKind.Action => "Pointer", // Callback pointer
            InteropTypeKind.Func => "Pointer", // Callback pointer
            InteropTypeKind.Unknown => "Pointer",
            _ => "Pointer"
        };
    }

    private string GetJavaIntegerType(InteropTypeModel type)
    {
        var typeName = type.OriginalTypeName ?? "";
        if (typeName.Contains("Int64") || typeName.Contains("UInt64"))
            return "long";
        if (typeName.Contains("Int16") || typeName.Contains("UInt16"))
            return "short";
        if (typeName.Contains("Byte") || typeName.Contains("SByte"))
            return "byte";
        return "int";
    }

    private string GetJnaIntegerType(InteropTypeModel type)
    {
        var typeName = type.OriginalTypeName ?? "";
        if (typeName.Contains("Int64") || typeName.Contains("UInt64"))
            return "long";
        if (typeName.Contains("Int16") || typeName.Contains("UInt16"))
            return "short";
        if (typeName.Contains("Byte") || typeName.Contains("SByte"))
            return "byte";
        return "int";
    }

    private string GetJavaFloatType(InteropTypeModel type)
    {
        return type.OriginalTypeName?.Contains("Double") == true ? "double" : "float";
    }

    private string GetJnaFloatType(InteropTypeModel type)
    {
        return type.OriginalTypeName?.Contains("Double") == true ? "double" : "float";
    }

    private string FormatFunctionalInterface(InteropTypeModel type, bool isFunc)
    {
        if (type.TypeArguments == null || type.TypeArguments.Length == 0)
            return isFunc ? "Supplier<?>" : "Runnable";

        if (isFunc)
        {
            if (type.TypeArguments.Length == 2)
                return $"Function<{GetTargetTypeBoxed(type.TypeArguments[0])}, {GetTargetTypeBoxed(type.TypeArguments[1])}>";
            return "Function<?, ?>";
        }
        else
        {
            if (type.TypeArguments.Length == 1)
                return $"Consumer<{GetTargetTypeBoxed(type.TypeArguments[0])}>";
            return "Consumer<?>";
        }
    }

    /// <summary>
    /// Gets the boxed type name for use in generics (Integer instead of int).
    /// </summary>
    private string GetTargetTypeBoxed(InteropTypeModel type)
    {
        var targetType = GetTargetType(type);
        return targetType switch
        {
            "int" => "Integer",
            "long" => "Long",
            "short" => "Short",
            "byte" => "Byte",
            "float" => "Float",
            "double" => "Double",
            "boolean" => "Boolean",
            "void" => "Void",
            _ => targetType
        };
    }

    public string FormatDefaultValue(InteropParameterModel parameter)
    {
        // Java doesn't support default parameter values directly
        // Overloaded methods are used instead
        return null;
    }

    public string GetInteropValue(InteropParameterModel parameter, string variableName)
    {
        return parameter.Type.Kind switch
        {
            InteropTypeKind.Enum => $"{variableName}.getValue()",
            InteropTypeKind.String => $"new WString({variableName})",
            InteropTypeKind.Boolean => $"({variableName} ? (byte)1 : (byte)0)",
            InteropTypeKind.Color => $"{variableName}.getHex()",
            InteropTypeKind.Action => $"{variableName}Callback",
            InteropTypeKind.Func => $"{variableName}Callback",
            InteropTypeKind.Class => $"{variableName}.getPointer()",
            InteropTypeKind.Interface => $"{variableName}.getPointer()",
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
        // Build Java method parameter string
        var javaParams = method.Parameters.Select(p =>
        {
            var name = ConvertName(p.OriginalName, NameContext.Parameter);
            var type = GetTargetType(p.Type);
            return new { Name = name, Type = type, OriginalType = p.Type };
        }).ToList();

        var javaParametersStr = string.Join(", ", javaParams.Select(p => $"{p.Type} {p.Name}"));

        // Build native call arguments
        var nativeArgs = new List<string> { "this.ptr" };
        nativeArgs.AddRange(method.Parameters.Select(p =>
            GetInteropValue(p, ConvertName(p.OriginalName, NameContext.Parameter))));
        var nativeCallArgsStr = string.Join(", ", nativeArgs);

        // Build JNA interface method signature
        var jnaSignature = BuildJnaSignature(method);

        // Determine return type
        var javaReturnType = GetReturnTypeName(method);
        var jnaReturnType = method.ReturnType.Kind == InteropTypeKind.Void
            ? "void"
            : GetJnaType(method.ReturnType);
        var returnClassName = javaReturnType != "void" ? javaReturnType : null;

        // Extract unique ID from native entry point
        var uniqueId = ExtractUniqueId(method.NativeEntryPoint);

        // Use disambiguated name for overloads
        var methodName = method.IsOverload
            ? ConvertName(method.DisambiguatedName, NameContext.Method)
            : ConvertName(method.OriginalName, NameContext.Method);

        return new
        {
            JavaMethodName = methodName,
            NativeMethodName = method.NativeEntryPoint,
            JavaParameters = javaParametersStr,
            JavaReturnType = javaReturnType,
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
            Parameters = javaParams.Select(p => new
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
            return "void";

        if (method.ReturnType.Kind == InteropTypeKind.TypeParameter)
            return _currentClassName;

        return GetTargetType(method.ReturnType);
    }

    private string BuildJnaSignature(InteropMethodModel method)
    {
        var returnType = GetJnaType(method.ReturnType);
        var methodName = method.NativeEntryPoint;

        // Build parameters: first is always Pointer target
        var parameters = new List<string> { "Pointer target" };
        parameters.AddRange(method.Parameters.Select(p =>
        {
            var jnaType = GetJnaType(p.Type);
            return $"{jnaType} {p.OriginalName}";
        }));

        return $"{returnType} {methodName}({string.Join(", ", parameters)})";
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
