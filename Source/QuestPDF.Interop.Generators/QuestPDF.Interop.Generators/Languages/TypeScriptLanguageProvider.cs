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
            InteropTypeKind.TypeParameter => "unknown",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Action => FormatFunctionType(type, isFunc: false),
            InteropTypeKind.Func => FormatFunctionType(type, isFunc: true),
            InteropTypeKind.Unknown => "unknown",
            _ => "unknown"
        };
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
            InteropTypeKind.Action => $"{variableName}Callback",
            InteropTypeKind.Func => $"{variableName}Callback",
            _ => variableName
        };
    }

    public object BuildClassTemplateModel(InteropClassModel classModel)
    {
        return new
        {
            ClassName = classModel.GeneratedClassName,
            Methods = classModel.Methods.Select(BuildMethodTemplateModel).ToList()
        };
    }

    private object BuildMethodTemplateModel(InteropMethodModel method)
    {
        return new
        {
            TsMethodName = ConvertName(method.OriginalName, NameContext.Method),
            NativeMethodName = method.NativeEntryPoint,
            Parameters = method.Parameters.Select(p => new
            {
                Name = ConvertName(p.OriginalName, NameContext.Parameter),
                Type = GetTargetType(p.Type),
                HasDefault = p.HasDefaultValue,
                DefaultValue = FormatDefaultValue(p)
            }).ToList(),
            ReturnType = GetTargetType(method.ReturnType),
            DeprecationMessage = method.DeprecationMessage,
            Callbacks = method.Callbacks.Select(c => new
            {
                ParameterName = ConvertName(c.ParameterName, NameContext.Parameter),
                ArgumentTypeName = c.ArgumentTypeName
            }).ToList()
        };
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
