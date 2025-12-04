using System.Collections.Generic;
using System.Linq;
using QuestPDF.Interop.Generators.Models;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Language provider for Python code generation.
/// </summary>
public class PythonLanguageProvider : ILanguageProvider
{
    public string LanguageName => "Python";
    public string FileExtension => ".py";
    public string MainTemplateName => "Python.Main";
    public string ObjectTemplateName => "Python.Object";
    public string EnumTemplateName => "Python.Enum";
    public string ColorsTemplateName => "Python.Colors";

    public string ConvertName(string csharpName, NameContext context)
    {
        return context switch
        {
            NameContext.Method => csharpName.ToSnakeCase(),
            NameContext.Parameter => csharpName.ToSnakeCase(),
            NameContext.EnumValue => csharpName.ToSnakeCase(),
            NameContext.Property => csharpName.ToSnakeCase(),
            NameContext.Constant => csharpName.ToSnakeCase(),
            NameContext.Class => csharpName, // Keep PascalCase for classes in Python
            _ => csharpName
        };
    }

    public string GetTargetType(InteropTypeModel type)
    {
        return type.Kind switch
        {
            InteropTypeKind.Void => "None",
            InteropTypeKind.Boolean => "bool",
            InteropTypeKind.Integer => "int",
            InteropTypeKind.Float => "float",
            InteropTypeKind.String => "str",
            InteropTypeKind.Enum => type.ShortName,
            InteropTypeKind.Class => type.ShortName,
            InteropTypeKind.Interface => type.ShortName.TrimStart('I'),
            InteropTypeKind.TypeParameter => "Any",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Action => FormatCallableType(type, isFunc: false),
            InteropTypeKind.Func => FormatCallableType(type, isFunc: true),
            InteropTypeKind.Unknown => "Any",
            _ => "Any"
        };
    }

    private string FormatCallableType(InteropTypeModel type, bool isFunc)
    {
        if (type.TypeArguments == null || type.TypeArguments.Length == 0)
            return isFunc ? "Callable[[], Any]" : "Callable[[], None]";

        if (isFunc)
        {
            var args = type.TypeArguments.Take(type.TypeArguments.Length - 1)
                .Select(t => $"'{GetTargetTypeForCallable(t)}'");
            var returnType = GetTargetTypeForCallable(type.TypeArguments.Last());
            return $"Callable[[{string.Join(", ", args)}], '{returnType}']";
        }
        else
        {
            var args = type.TypeArguments.Select(t => $"'{GetTargetTypeForCallable(t)}'");
            return $"Callable[[{string.Join(", ", args)}], None]";
        }
    }

    private string GetTargetTypeForCallable(InteropTypeModel type)
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
            return "None";

        if (parameter.Type.Kind == InteropTypeKind.Enum && parameter.DefaultEnumMemberName != null)
            return $"{parameter.Type.ShortName}.{parameter.DefaultEnumMemberName.ToSnakeCase()}";

        if (parameter.DefaultValue is bool boolValue)
            return boolValue ? "True" : "False";

        if (parameter.DefaultValue is string stringValue)
            return $"'{stringValue}'";

        return parameter.DefaultValue?.ToString() ?? "None";
    }

    public string GetInteropValue(InteropParameterModel parameter, string variableName)
    {
        if (parameter.Type.InteropType.Contains("QuestPDF.Infrastructure.Color"))
            return $"{variableName}.hex";
        
        return parameter.Type.Kind switch
        {
            InteropTypeKind.Enum => $"{variableName}.value",
            InteropTypeKind.String => $"questpdf_ffi.new(\"char[]\", {variableName}.encode(\"utf-8\"))",
            InteropTypeKind.Action => $"_internal_{variableName}_handler",
            InteropTypeKind.Func => $"_internal_{variableName}_handler",
            _ => variableName
        };
    }

    // Store the current class name for resolving TypeParameter return types
    private string _currentClassName;

    public object BuildClassTemplateModel(InteropClassModel classModel, string customInit, string customClass)
    {
        _currentClassName = classModel.GeneratedClassName;

        return new
        {
            CallbackTypedefs = classModel.CallbackTypedefs,
            Headers = classModel.CHeaderSignatures,
            ClassName = classModel.GeneratedClassName,
            Methods = classModel.Methods.Select(BuildMethodTemplateModel).ToList(),
            CustomInit = customInit,
            CustomClass = customClass
        };
    }

    private object BuildMethodTemplateModel(InteropMethodModel method)
    {
        var parameters = new List<string>();
        
        if (!method.IsStaticMethod)
            parameters.Add("self");
        
        parameters.AddRange(method.Parameters.Select(FormatParameter));

        var interopParams = new List<string>();
        
        if (!method.IsStaticMethod)
            interopParams.Add("self.target_pointer");
        
        interopParams.AddRange(method.Parameters.Select(p =>
            GetInteropValue(p, ConvertName(p.OriginalName, NameContext.Parameter))));

        // Use disambiguated name for overloads (makes them private with _prefix in template)
        var methodName = method.IsOverload
            ? "_" + ConvertName(method.DisambiguatedName, NameContext.Method)
            : ConvertName(method.OriginalName, NameContext.Method);

        return new
        {
            PythonMethodName = methodName,
            PythonMethodParameters = parameters,
            IsStaticMethod = method.IsStaticMethod,
            InteropMethodName = method.NativeEntryPoint,
            InteropMethodParameters = interopParams,
            PythonMethodReturnType = GetReturnTypeName(method),
            DeprecationMessage = method.DeprecationMessage,
            Callbacks = method.Callbacks.Select(BuildCallbackTemplateModel).ToList(),
            IsOverload = method.IsOverload,
            OriginalName = ConvertName(method.OriginalName, NameContext.Method)
        };
    }

    private string FormatParameter(InteropParameterModel parameter)
    {
        var name = ConvertName(parameter.OriginalName, NameContext.Parameter);
        var typeName = GetTargetType(parameter.Type);
        var result = $"{name}: {typeName}";

        if (parameter.HasDefaultValue)
            result += $" = {FormatDefaultValue(parameter)}";

        return result;
    }

    private string GetReturnTypeName(InteropMethodModel method)
    {
        if (method.ReturnType.Kind == InteropTypeKind.Void)
            return null;

        // For TypeParameter (generic methods returning T), use the containing class name
        if (method.ReturnType.Kind == InteropTypeKind.TypeParameter)
            return _currentClassName;

        return GetTargetType(method.ReturnType);
    }

    private object BuildCallbackTemplateModel(InteropCallbackModel callback)
    {
        return new
        {
            PythonParameterName = ConvertName(callback.ParameterName, NameContext.Parameter),
            CallbackArgumentTypeName = callback.ArgumentTypeName,
            InternalCallbackName = $"_internal_{ConvertName(callback.ParameterName, NameContext.Parameter)}_handler"
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
