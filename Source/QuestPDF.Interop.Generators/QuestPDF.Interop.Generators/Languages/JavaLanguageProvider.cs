using System.Collections.Generic;
using System.Linq;
using QuestPDF.Interop.Generators.Models;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Language provider for Java code generation using JNA.
/// </summary>
public class JavaLanguageProvider : ILanguageProvider
{
    public string LanguageName => "Java";
    public string FileExtension => ".java";
    public string MainTemplateName => "Java.Main";
    public string ObjectTemplateName => "Java.Object";
    public string EnumTemplateName => "Java.Enum";
    public string ColorsTemplateName => "Java.Colors";

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
            InteropTypeKind.TypeParameter => "Object",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Action => FormatFunctionalInterface(type, isFunc: false),
            InteropTypeKind.Func => FormatFunctionalInterface(type, isFunc: true),
            InteropTypeKind.Unknown => "Object",
            _ => "Object"
        };
    }

    private string GetJavaIntegerType(InteropTypeModel type)
    {
        var typeName = type.OriginalTypeName;
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
        return type.OriginalTypeName.Contains("Double") ? "double" : "float";
    }

    private string FormatFunctionalInterface(InteropTypeModel type, bool isFunc)
    {
        if (type.TypeArguments == null || type.TypeArguments.Length == 0)
            return isFunc ? "Supplier<?>" : "Runnable";

        if (isFunc)
        {
            if (type.TypeArguments.Length == 2)
                return $"Function<{GetTargetTypeForGeneric(type.TypeArguments[0])}, {GetTargetTypeForGeneric(type.TypeArguments[1])}>";
            return "Function<?, ?>";
        }
        else
        {
            if (type.TypeArguments.Length == 1)
                return $"Consumer<{GetTargetTypeForGeneric(type.TypeArguments[0])}>";
            return "Consumer<?>";
        }
    }

    private string GetTargetTypeForGeneric(InteropTypeModel type)
    {
        if (type.Kind == InteropTypeKind.Interface)
            return type.ShortName.TrimStart('I');
        return type.ShortName;
    }

    public string FormatDefaultValue(InteropParameterModel parameter)
    {
        // Java doesn't support default parameter values in the same way
        // We would need to use method overloading
        return null;
    }

    public string GetInteropValue(InteropParameterModel parameter, string variableName)
    {
        return parameter.Type.Kind switch
        {
            InteropTypeKind.Enum => $"{variableName}.getValue()",
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
            JavaMethodName = ConvertName(method.OriginalName, NameContext.Method),
            NativeMethodName = method.NativeEntryPoint,
            Parameters = method.Parameters.Select(p => new
            {
                Name = ConvertName(p.OriginalName, NameContext.Parameter),
                Type = GetTargetType(p.Type)
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
