using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Base class for language-specific code generation providers.
/// Implements the shared logic for building template models,
/// delegating language-specific details to abstract methods.
/// </summary>
internal abstract class LanguageProviderBase : ILanguageProvider
{
    // ═══════════════════════════════════════════════════════════════════
    // Abstract: each language must define these
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>Name to include in the parameter list for instance methods (e.g. "self" in Python, null for others).</summary>
    protected abstract string SelfParameterName { get; }

    /// <summary>Expression to pass as the target pointer in native calls (e.g. "self.target_pointer", "this._ptr", "pointer").</summary>
    protected abstract string SelfPointerExpression { get; }

    /// <summary>Prefix added to overloaded method names (e.g. "_" in Python to make them private by convention).</summary>
    protected virtual string OverloadMethodNamePrefix => "";

    /// <summary>Converts a C# name to the target language's naming convention.</summary>
    public abstract string ConvertName(string name, NameContext context);

    /// <summary>Maps a Roslyn type symbol to the target language's type name.</summary>
    protected abstract string GetTargetType(ITypeSymbol type);

    /// <summary>Formats a parameter's default value in the target language's syntax.</summary>
    protected abstract string FormatDefaultValue(IParameterSymbol parameter);

    /// <summary>Produces the expression to pass a parameter value across the FFI boundary.</summary>
    protected abstract string GetInteropValue(IParameterSymbol parameter, string variableName);

    // ═══════════════════════════════════════════════════════════════════
    // Virtual: override for language-specific extras (null = not applicable)
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>Builds the native function signature string (e.g. koffi C signature, JNA Kotlin signature).</summary>
    protected virtual string BuildNativeSignature(IMethodSymbol method, INamedTypeSymbol targetType) => null;

    /// <summary>Gets the native/FFI return type (e.g. JNA return type for Kotlin).</summary>
    protected virtual string GetNativeReturnType(IMethodSymbol method) => null;

    /// <summary>Gets the return conversion method name (e.g. "decode_text_as_utf_8" in Python).</summary>
    protected virtual string GetReturnConversionMethod(IMethodSymbol method, string className) => null;

    /// <summary>Builds per-parameter detail models for native interface declarations (Kotlin JNA).</summary>
    protected virtual List<object> BuildParameterDetails(IReadOnlyList<IParameterSymbol> parameters) => null;

    /// <summary>Builds callback interface models (Kotlin JNA).</summary>
    protected virtual List<object> BuildCallbackInterfaces(IReadOnlyList<IMethodSymbol> methods) => null;

    /// <summary>Builds C header definition strings for native function declarations (Python cffi, Kotlin).</summary>
    protected virtual List<string> BuildHeaders(IReadOnlyList<IMethodSymbol> methods, string className) => null;

    // ═══════════════════════════════════════════════════════════════════
    // Shared: BuildClassTemplateModel (orchestration)
    // ═══════════════════════════════════════════════════════════════════

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

    // ═══════════════════════════════════════════════════════════════════
    // Shared: BuildMethodModel
    // ═══════════════════════════════════════════════════════════════════

    private object BuildMethodModel(
        IMethodSymbol method, INamedTypeSymbol targetType,
        Dictionary<IMethodSymbol, OverloadInfo> overloads, string className)
    {
        var isStaticMethod = method.IsStatic && !method.IsExtensionMethod;
        var nonThisParams = method.GetNonThisParameters();
        var overloadInfo = overloads[method];
        var nativeEntryPoint = method.GetNativeMethodName(className);
        var uniqueId = nativeEntryPoint.ExtractNativeMethodHash();

        // Determine method name (with overload disambiguation)
        var methodName = overloadInfo.IsOverload
            ? OverloadMethodNamePrefix + ConvertName(overloadInfo.DisambiguatedName, NameContext.Method)
            : ConvertName(method.Name, NameContext.Method);

        // Build formatted parameters string
        var paramParts = nonThisParams.Select(FormatParameter).ToList();
        if (!isStaticMethod && SelfParameterName != null)
            paramParts.Insert(0, SelfParameterName);
        var parameters = string.Join(", ", paramParts);

        // Build native call arguments string
        var nativeArgParts = nonThisParams
            .Select(p => GetInteropValue(p, ConvertName(p.Name, NameContext.Parameter)))
            .ToList();
        if (!isStaticMethod)
            nativeArgParts.Insert(0, SelfPointerExpression);
        var nativeCallArgs = string.Join(", ", nativeArgParts);

        // Return type info
        var returnType = GetReturnTypeName(method, className);
        var returnTypeKind = method.ReturnType.GetInteropTypeKind();
        var isVoidReturn = returnTypeKind == InteropTypeKind.Void;

        return new
        {
            MethodName = methodName,
            NativeMethodName = nativeEntryPoint,
            Parameters = parameters,
            ReturnType = returnType,
            ReturnClassName = isVoidReturn ? null : returnType,
            NativeCallArgs = nativeCallArgs,
            NativeSignature = BuildNativeSignature(method, targetType),
            NativeReturnType = GetNativeReturnType(method),
            ReturnConversionMethod = GetReturnConversionMethod(method, className),
            UniqueId = uniqueId,
            IsStaticMethod = isStaticMethod,
            DeprecationMessage = method.TryGetDeprecationMessage(),
            Callbacks = method.GetCallbackParameters().Select(c => BuildCallbackModel(c, uniqueId)).ToList(),
            IsOverload = overloadInfo.IsOverload,
            OriginalName = ConvertName(method.Name, NameContext.Method),
            ParameterDetails = BuildParameterDetails(nonThisParams),
            ReturnsPointer = returnTypeKind is InteropTypeKind.Class or InteropTypeKind.Interface or InteropTypeKind.TypeParameter,
            HasCallbacks = method.GetCallbackParameters().Any(),
        };
    }

    // ═══════════════════════════════════════════════════════════════════
    // Shared: helper methods
    // ═══════════════════════════════════════════════════════════════════

    protected string GetReturnTypeName(IMethodSymbol method, string className)
    {
        var kind = method.ReturnType.GetInteropTypeKind();
        if (kind == InteropTypeKind.TypeParameter)
            return className;
        return GetTargetType(method.ReturnType);
    }

    protected string GetTargetTypeForCallback(ITypeSymbol type)
    {
        if (type.GetInteropTypeKind() == InteropTypeKind.Interface)
            return type.Name.TrimStart('I');
        return type.Name;
    }

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
