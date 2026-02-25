using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

internal enum NameContext
{
    Method,
    Parameter,
    Class,
    EnumValue,
    Property,
    Constant
}

internal abstract class LanguageProviderBase
{
    protected abstract string SelfParameterName { get; }
    protected abstract string SelfPointerExpression { get; }
    protected virtual string OverloadMethodNamePrefix => "";

    protected abstract string ConvertName(string name, NameContext context);
    protected abstract string GetTargetType(ITypeSymbol type);
    protected abstract string FormatDefaultValue(IParameterSymbol parameter);
    protected abstract string GetInteropValue(IParameterSymbol parameter, string variableName);

    protected virtual string? BuildNativeSignature(IMethodSymbol method, INamedTypeSymbol targetType) => null;
    protected virtual string? GetNativeReturnType(IMethodSymbol method) => null;
    protected virtual string? GetReturnConversionMethod(IMethodSymbol method, string className) => null;
    protected virtual List<object>? BuildParameterDetails(IReadOnlyList<IParameterSymbol> parameters) => null;
    protected virtual List<object>? BuildCallbackInterfaces(IReadOnlyList<IMethodSymbol> methods) => null;
    protected virtual List<string>? BuildHeaders(IReadOnlyList<IMethodSymbol> methods, string className) => null;

    public object BuildClassTemplateModel(
        INamedTypeSymbol targetType,
        IReadOnlyList<IMethodSymbol> methods,
        Dictionary<IMethodSymbol, OverloadInfo> overloads,
        string inheritFrom,
        string customDefinitions, string customInit, string customClass)
    {
        var className = targetType.GetGeneratedClassName();

        var callbackTypedefs = methods
            .GetCallbackTypedefs()
            .Select(t => t.TypedefDefinition)
            .Distinct()
            .ToList();

        return new
        {
            ClassName = className,
            InheritFrom = inheritFrom,
            Methods = methods.Select(m => BuildMethodModel(m, targetType, overloads, className)).ToList(),
            CallbackTypedefs = callbackTypedefs,
            Headers = BuildHeaders(methods, className),
            CallbackInterfaces = BuildCallbackInterfaces(methods),
            CustomDefinitions = customDefinitions,
            CustomInit = customInit,
            CustomClass = customClass
        };
    }

    private object BuildMethodModel(
        IMethodSymbol method, INamedTypeSymbol targetType,
        Dictionary<IMethodSymbol, OverloadInfo> overloads, string className)
    {
        var isStaticMethod = method.IsStatic && !method.IsExtensionMethod;
        var nonThisParams = method.GetNonThisParameters();
        var overloadInfo = overloads[method];
        var nativeEntryPoint = method.GetNativeMethodName(className);
        var uniqueId = method.ToDisplayString().GetDeterministicHash();

        var methodName = overloadInfo.IsOverload
            ? OverloadMethodNamePrefix + ConvertName(overloadInfo.DisambiguatedName, NameContext.Method)
            : ConvertName(method.Name, NameContext.Method);

        var paramParts = nonThisParams.Select(FormatParameter).ToList();
        
        if (!isStaticMethod && SelfParameterName != null)
            paramParts.Insert(0, SelfParameterName);

        var nativeArgParts = nonThisParams
            .Select(p => GetInteropValue(p, ConvertName(p.Name, NameContext.Parameter)))
            .ToList();
        
        if (!isStaticMethod)
            nativeArgParts.Insert(0, SelfPointerExpression);

        var returnType = GetReturnTypeName(method, className);
        var returnTypeKind = method.ReturnType.GetInteropTypeKind();
        
        var callbacks = method
            .GetCallbackParameters()
            .Select(c => BuildCallbackModel(c, uniqueId))
            .ToList();

        return new
        {
            MethodName = methodName,
            NativeMethodName = nativeEntryPoint,
            Parameters = string.Join(", ", paramParts),
            ReturnType = returnType,
            ReturnClassName = returnTypeKind == InteropTypeKind.Void ? null : returnType,
            NativeCallArgs = string.Join(", ", nativeArgParts),
            NativeSignature = BuildNativeSignature(method, targetType),
            NativeReturnType = GetNativeReturnType(method),
            ReturnConversionMethod = GetReturnConversionMethod(method, className),
            UniqueId = uniqueId,
            IsStaticMethod = isStaticMethod,
            DeprecationMessage = method.TryGetDeprecationMessage(),
            Callbacks = callbacks,
            HasCallbacks = callbacks.Count > 0,
            IsOverload = overloadInfo.IsOverload,
            OriginalName = ConvertName(method.Name, NameContext.Method),
            ParameterDetails = BuildParameterDetails(nonThisParams),
            ReturnsPointer = returnTypeKind is InteropTypeKind.Class or InteropTypeKind.Interface or InteropTypeKind.TypeParameter,
        };
    }

    protected string GetReturnTypeName(IMethodSymbol method, string className)
    {
        var kind = method.ReturnType.GetInteropTypeKind();
        
        if (kind == InteropTypeKind.TypeParameter)
            return className;
        
        return GetTargetType(method.ReturnType);
    }

    protected static string GetTargetTypeForCallback(ITypeSymbol type) => type.GetInteropTypeName();

    protected string FormatParameter(IParameterSymbol parameter)
    {
        var name = ConvertName(parameter.Name, NameContext.Parameter);
        var typeName = GetTargetType(parameter.Type);
        var result = $"{name}: {typeName}";

        if (parameter.HasExplicitDefaultValue)
            result += $" = {FormatDefaultValue(parameter)}";

        return result;
    }

    private object BuildCallbackModel(IParameterSymbol callbackParam, string uniqueId)
    {
        var parameterName = ConvertName(callbackParam.Name, NameContext.Parameter);
        var argumentTypeName = callbackParam.GetCallbackArgumentTypeName();

        return new
        {
            ParameterName = parameterName,
            ArgumentTypeName = argumentTypeName,
            InternalCallbackName = $"_internal_{parameterName}_handler",
            CallbackInterfaceName = $"{argumentTypeName}Callback",
            UniqueId = uniqueId
        };
    }
}
