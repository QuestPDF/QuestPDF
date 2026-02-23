using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Language provider for Python code generation.
/// </summary>
internal class PythonLanguageProvider : ILanguageProvider
{
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

    public string GetTargetType(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Void => "None",
            InteropTypeKind.Boolean => "bool",
            InteropTypeKind.Integer => "int",
            InteropTypeKind.Float => "float",
            InteropTypeKind.String => "str",
            InteropTypeKind.ByteArray => "bytes",
            InteropTypeKind.Size => "Size",
            InteropTypeKind.ImageSize => "ImageSize",
            InteropTypeKind.Enum => type.Name,
            InteropTypeKind.Class => type.Name,
            InteropTypeKind.Interface => type.Name.TrimStart('I'),
            InteropTypeKind.TypeParameter => "Any",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Action => FormatCallableType((INamedTypeSymbol)type, isFunc: false),
            InteropTypeKind.Func => FormatCallableType((INamedTypeSymbol)type, isFunc: true),
            InteropTypeKind.Unknown => "Any",
            _ => "Any"
        };
    }

    private string FormatCallableType(INamedTypeSymbol type, bool isFunc)
    {
        if (type.TypeArguments.Length == 0)
            return "Callable[[], Any]"; // both Action and Func expect Any to mitigate problems with multi-line lambdas

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
            return $"Callable[[{string.Join(", ", args)}], Any]";
        }
    }

    private string GetTargetTypeForCallable(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();

        if (kind == InteropTypeKind.Interface)
            return type.Name.TrimStart('I');

        if (kind == InteropTypeKind.ByteArray)
            return "bytes";

        if (kind == InteropTypeKind.ImageSize)
            return "ImageSize";

        if (kind == InteropTypeKind.Size)
            return "Size";

        if (kind == InteropTypeKind.String)
            return "str";

        return type.Name;
    }

    public string FormatDefaultValue(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        if (parameter.ExplicitDefaultValue == null)
            return "None";

        var kind = parameter.Type.GetInteropTypeKind();

        if (kind == InteropTypeKind.Enum && parameter.GetDefaultEnumMemberName() != null)
            return $"{parameter.Type.Name}.{parameter.GetDefaultEnumMemberName().ToSnakeCase()}";

        if (parameter.ExplicitDefaultValue is bool boolValue)
            return boolValue ? "True" : "False";

        if (parameter.ExplicitDefaultValue is string stringValue)
            return $"'{stringValue}'";

        return parameter.ExplicitDefaultValue?.ToString() ?? "None";
    }

    public string GetInteropValue(IParameterSymbol parameter, string variableName)
    {
        var kind = parameter.Type.GetInteropTypeKind();

        if (kind == InteropTypeKind.Color)
            return $"{variableName}.hex";

        return kind switch
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

    public object BuildClassTemplateModel(
        INamedTypeSymbol targetType,
        IReadOnlyList<IMethodSymbol> methods,
        Dictionary<IMethodSymbol, OverloadInfo> overloads,
        string inheritFrom,
        string customDefinitions, string customInit, string customClass)
    {
        var className = targetType.GetGeneratedClassName();
        _currentClassName = className;

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
            CallbackTypedefs = callbackTypedefs,
            InheritFrom = inheritFrom,
            Headers = cHeaders,
            ClassName = className,
            Methods = methods.Select(m => BuildMethodTemplateModel(m, targetType, overloads)).ToList(),
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

        var parameters = new List<string>();

        if (!isStaticMethod)
            parameters.Add("self");

        parameters.AddRange(nonThisParams.Select(FormatParameter));

        var interopParams = new List<string>();

        if (!isStaticMethod)
            interopParams.Add("self.target_pointer");

        interopParams.AddRange(nonThisParams.Select(p =>
            GetInteropValue(p, ConvertName(p.Name, NameContext.Parameter))));

        // Use disambiguated name for overloads (makes them private with _prefix in template)
        var methodName = overloadInfo.IsOverload
            ? "_" + ConvertName(overloadInfo.DisambiguatedName, NameContext.Method)
            : ConvertName(method.Name, NameContext.Method);

        return new
        {
            PythonMethodName = methodName,
            PythonMethodParameters = parameters,
            IsStaticMethod = isStaticMethod,
            InteropMethodName = method.GetNativeMethodName(className),
            InteropMethodParameters = interopParams,
            PythonMethodReturnType = GetReturnTypeName(method),
            PythonReturnConversionMethod = GetReturnConversionMethod(method),
            DeprecationMessage = method.TryGetDeprecationMessage(),
            Callbacks = method.GetCallbackParameters().Select(BuildCallbackTemplateModel).ToList(),
            IsOverload = overloadInfo.IsOverload,
            OriginalName = ConvertName(method.Name, NameContext.Method)
        };
    }

    private string FormatParameter(IParameterSymbol parameter)
    {
        var name = ConvertName(parameter.Name, NameContext.Parameter);
        var typeName = GetTargetType(parameter.Type);
        var result = $"{name}: {typeName}";

        if (parameter.HasExplicitDefaultValue)
            result += $" = {FormatDefaultValue(parameter)}";

        return result;
    }

    private string GetReturnTypeName(IMethodSymbol method)
    {
        var kind = method.ReturnType.GetInteropTypeKind();

        if (kind == InteropTypeKind.Void)
            return null;

        // For TypeParameter (generic methods returning T), use the containing class name
        if (kind == InteropTypeKind.TypeParameter)
            return _currentClassName;

        return GetTargetType(method.ReturnType);
    }

    private string GetReturnConversionMethod(IMethodSymbol method)
    {
        var kind = method.ReturnType.GetInteropTypeKind();

        if (kind == InteropTypeKind.Void)
            return null;

        if (kind == InteropTypeKind.String)
            return "decode_text_as_utf_8";

        // For TypeParameter (generic methods returning T), use the containing class name
        if (kind == InteropTypeKind.TypeParameter)
            return _currentClassName;

        if (kind == InteropTypeKind.ByteArray)
            return "questpdf_ffi.string";

        return GetTargetType(method.ReturnType);
    }


    private object BuildCallbackTemplateModel(IParameterSymbol callbackParam)
    {
        return new
        {
            PythonParameterName = ConvertName(callbackParam.Name, NameContext.Parameter),
            CallbackArgumentTypeName = callbackParam.GetCallbackArgumentTypeName(),
            InternalCallbackName = $"_internal_{ConvertName(callbackParam.Name, NameContext.Parameter)}_handler"
        };
    }
}
