using System.Collections.Generic;
using System.Linq;
using QuestPDF.Interop.Generators.Models;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Language provider for TypeScript code generation using koffi.
/// </summary>
public class TypeScriptLanguageProvider : ILanguageProvider
{
    public string LanguageName => "TypeScript";
    public string FileExtension => ".ts";
    public string MainTemplateName => "TypeScript.Main";
    public string ObjectTemplateName => "TypeScript.Object";
    public string EnumTemplateName => "TypeScript.Enum";
    public string ColorsTemplateName => "TypeScript.Colors";

    // Store current class name for TypeParameter resolution
    private string _currentClassName;

    public string ConvertName(string csharpName, NameContext context)
    {
        return context switch
        {
            NameContext.Method => ToCamelCase(csharpName),
            NameContext.Parameter => ToCamelCase(csharpName),
            NameContext.EnumValue => csharpName, // Keep PascalCase for enum values in TypeScript
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
            InteropTypeKind.Integer => "number",
            InteropTypeKind.Float => "number",
            InteropTypeKind.String => "string",
            InteropTypeKind.Enum => type.ShortName,
            InteropTypeKind.Class => type.ShortName,
            InteropTypeKind.Interface => type.ShortName.TrimStart('I'),
            InteropTypeKind.TypeParameter => _currentClassName ?? "unknown",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Action => FormatFunctionType(type, isFunc: false),
            InteropTypeKind.Func => FormatFunctionType(type, isFunc: true),
            InteropTypeKind.Unknown => "unknown",
            _ => "unknown"
        };
    }

    /// <summary>
    /// Gets the koffi-compatible C type for FFI declarations.
    /// </summary>
    public string GetKoffiType(InteropTypeModel type)
    {
        return type.Kind switch
        {
            InteropTypeKind.Void => "void",
            InteropTypeKind.Boolean => "uint8_t",
            InteropTypeKind.Integer => GetKoffiIntegerType(type),
            InteropTypeKind.Float => GetKoffiFloatType(type),
            InteropTypeKind.String => "const char*",
            InteropTypeKind.Enum => "int32_t",
            InteropTypeKind.Class => "void*",
            InteropTypeKind.Interface => "void*",
            InteropTypeKind.TypeParameter => "void*",
            InteropTypeKind.Color => "uint32_t",
            InteropTypeKind.Action => "void*", // Callback pointer
            InteropTypeKind.Func => "void*", // Callback pointer
            InteropTypeKind.Unknown => "void*",
            _ => "void*"
        };
    }

    private string GetKoffiIntegerType(InteropTypeModel type)
    {
        var typeName = type.OriginalTypeName ?? "";
        if (typeName.Contains("Int64") || typeName.Contains("long"))
            return "int64_t";
        if (typeName.Contains("UInt64") || typeName.Contains("ulong"))
            return "uint64_t";
        if (typeName.Contains("Int16") || typeName.Contains("short"))
            return "int16_t";
        if (typeName.Contains("UInt16") || typeName.Contains("ushort"))
            return "uint16_t";
        if (typeName.Contains("Byte"))
            return "uint8_t";
        if (typeName.Contains("SByte"))
            return "int8_t";
        if (typeName.Contains("UInt32") || typeName.Contains("uint"))
            return "uint32_t";
        return "int32_t";
    }

    private string GetKoffiFloatType(InteropTypeModel type)
    {
        var typeName = type.OriginalTypeName ?? "";
        return typeName.Contains("Double") || typeName.Contains("double") ? "double" : "float";
    }

    private string FormatFunctionType(InteropTypeModel type, bool isFunc)
    {
        if (type.TypeArguments == null || type.TypeArguments.Length == 0)
            return isFunc ? "() => unknown" : "() => void";

        if (isFunc)
        {
            var args = type.TypeArguments.Take(type.TypeArguments.Length - 1)
                .Select((t, i) => $"arg{i}: {GetTargetTypeForCallback(t)}");
            var returnType = GetTargetTypeForCallback(type.TypeArguments.Last());
            return $"({string.Join(", ", args)}) => {returnType}";
        }
        else
        {
            var args = type.TypeArguments.Select((t, i) => $"arg{i}: {GetTargetTypeForCallback(t)}");
            return $"({string.Join(", ", args)}) => void";
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
            return "undefined";

        if (parameter.Type.Kind == InteropTypeKind.Enum && parameter.DefaultEnumMemberName != null)
            return $"{parameter.Type.ShortName}.{parameter.DefaultEnumMemberName}";

        if (parameter.DefaultValue is bool boolValue)
            return boolValue ? "true" : "false";

        if (parameter.DefaultValue is string stringValue)
            return $"'{stringValue}'";

        return parameter.DefaultValue?.ToString() ?? "undefined";
    }

    public string GetInteropValue(InteropParameterModel parameter, string variableName)
    {
        return parameter.Type.Kind switch
        {
            InteropTypeKind.Enum => variableName,
            InteropTypeKind.String => variableName,
            InteropTypeKind.Action => $"{variableName}Cb",
            InteropTypeKind.Func => $"{variableName}Cb",
            InteropTypeKind.Class => $"{variableName}.pointer",
            InteropTypeKind.Interface => $"{variableName}.pointer",
            _ => variableName
        };
    }

    public object BuildClassTemplateModel(InteropClassModel classModel)
    {
        _currentClassName = classModel.GeneratedClassName;

        return new
        {
            ClassName = classModel.GeneratedClassName,
            CallbackTypedefs = classModel.CallbackTypedefs,
            Methods = classModel.Methods.Select(BuildMethodTemplateModel).ToList()
        };
    }

    private object BuildMethodTemplateModel(InteropMethodModel method)
    {
        // Build TypeScript parameter string
        var tsParams = method.Parameters.Select(p =>
        {
            var name = ConvertName(p.OriginalName, NameContext.Parameter);
            var type = GetTargetType(p.Type);
            var defaultVal = p.HasDefaultValue ? FormatDefaultValue(p) : null;
            return defaultVal != null ? $"{name}: {type} = {defaultVal}" : $"{name}: {type}";
        });
        var tsParametersStr = string.Join(", ", tsParams);

        // Build native call arguments
        var nativeArgs = new List<string> { "this._ptr" };
        nativeArgs.AddRange(method.Parameters.Select(p =>
            GetInteropValue(p, ConvertName(p.OriginalName, NameContext.Parameter))));
        var nativeCallArgsStr = string.Join(", ", nativeArgs);

        // Build C signature for koffi.func()
        var cSignature = BuildCSignature(method);

        // Determine return type and class name
        var tsReturnType = GetReturnTypeName(method);
        var returnClassName = tsReturnType != "void" ? tsReturnType : null;

        return new
        {
            TsMethodName = ConvertName(method.OriginalName, NameContext.Method),
            NativeMethodName = method.NativeEntryPoint,
            TsParameters = tsParametersStr,
            TsReturnType = tsReturnType,
            ReturnClassName = returnClassName,
            CSignature = cSignature,
            NativeCallArgs = nativeCallArgsStr,
            DeprecationMessage = method.DeprecationMessage,
            Callbacks = method.Callbacks.Select(c => new
            {
                ParameterName = ConvertName(c.ParameterName, NameContext.Parameter),
                ArgumentTypeName = c.ArgumentTypeName
            }).ToList()
        };
    }

    private string GetReturnTypeName(InteropMethodModel method)
    {
        if (method.ReturnType.Kind == InteropTypeKind.Void)
            return "void";

        if (method.ReturnType.Kind == InteropTypeKind.TypeParameter)
            return _currentClassName;

        return GetTargetType(method.ReturnType);
    }

    private string BuildCSignature(InteropMethodModel method)
    {
        var returnType = GetKoffiType(method.ReturnType);
        var methodName = method.NativeEntryPoint;

        // Build parameters: first is always void* target
        var parameters = new List<string> { "void* target" };
        parameters.AddRange(method.Parameters.Select(p =>
        {
            var koffiType = GetKoffiType(p.Type);
            return $"{koffiType} {p.OriginalName}";
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
