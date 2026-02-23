using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Language provider for TypeScript code generation using koffi.
/// </summary>
internal class TypeScriptLanguageProvider : ILanguageProvider
{
    // Store current class name for TypeParameter resolution
    private string _currentClassName;

    public string ConvertName(string csharpName, NameContext context)
    {
        return context switch
        {
            NameContext.Method => csharpName.ToCamelCase(),
            NameContext.Parameter => csharpName.ToCamelCase(),
            NameContext.EnumValue => csharpName, // Keep PascalCase for enum values in TypeScript
            NameContext.Property => csharpName.ToCamelCase(),
            NameContext.Constant => csharpName.ToSnakeCase().ToUpperInvariant(),
            NameContext.Class => csharpName, // Keep PascalCase
            _ => csharpName
        };
    }

    public string GetTargetType(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Void => "void",
            InteropTypeKind.Boolean => "boolean",
            InteropTypeKind.Integer => "number",
            InteropTypeKind.Float => "number",
            InteropTypeKind.String => "string",
            InteropTypeKind.Enum => type.Name,
            InteropTypeKind.Class => type.Name,
            InteropTypeKind.Interface => type.Name.TrimStart('I'),
            InteropTypeKind.TypeParameter => _currentClassName ?? "unknown",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Action => FormatFunctionType((INamedTypeSymbol)type, isFunc: false),
            InteropTypeKind.Func => FormatFunctionType((INamedTypeSymbol)type, isFunc: true),
            InteropTypeKind.Unknown => "unknown",
            _ => "unknown"
        };
    }

    /// <summary>
    /// Gets the koffi-compatible C type for FFI declarations.
    /// Note: koffi doesn't support callback type names in signature strings,
    /// so all callbacks are declared as void* pointers.
    /// </summary>
    public string GetKoffiType(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Void => "void",
            InteropTypeKind.Boolean => "uint8_t",
            InteropTypeKind.Integer => GetKoffiIntegerType(type),
            InteropTypeKind.Float => GetKoffiFloatType(type),
            InteropTypeKind.String => "str16", // UTF-16 encoded strings for .NET compatibility
            InteropTypeKind.Enum => "int32_t",
            InteropTypeKind.Class => "void*",
            InteropTypeKind.Interface => "void*",
            InteropTypeKind.TypeParameter => "void*",
            InteropTypeKind.Color => "uint32_t",
            InteropTypeKind.Action => "void*", // Callback pointer - koffi handles conversion
            InteropTypeKind.Func => "void*", // Callback pointer - koffi handles conversion
            InteropTypeKind.Unknown => "void*",
            _ => "void*"
        };
    }

    private string GetKoffiIntegerType(ITypeSymbol type)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
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

    private string GetKoffiFloatType(ITypeSymbol type)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return typeName.Contains("Double") || typeName.Contains("double") ? "double" : "float";
    }

    private string FormatFunctionType(INamedTypeSymbol type, bool isFunc)
    {
        if (type.TypeArguments.Length == 0)
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

    private string GetTargetTypeForCallback(ITypeSymbol type)
    {
        if (type.GetInteropTypeKind() == InteropTypeKind.Interface)
            return type.Name.TrimStart('I');
        return type.Name;
    }

    public string FormatDefaultValue(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        if (parameter.ExplicitDefaultValue == null)
            return "undefined";

        var kind = parameter.Type.GetInteropTypeKind();

        if (kind == InteropTypeKind.Enum && parameter.GetDefaultEnumMemberName() != null)
            return $"{parameter.Type.Name}.{parameter.GetDefaultEnumMemberName()}";

        if (parameter.ExplicitDefaultValue is bool boolValue)
            return boolValue ? "true" : "false";

        if (parameter.ExplicitDefaultValue is string stringValue)
            return $"'{stringValue}'";

        return parameter.ExplicitDefaultValue?.ToString() ?? "undefined";
    }

    public string GetInteropValue(IParameterSymbol parameter, string variableName)
    {
        var kind = parameter.Type.GetInteropTypeKind();

        if (kind == InteropTypeKind.Color)
            return $"{variableName}.hex";

        return kind switch
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

        return new
        {
            ClassName = className,
            InheritFrom = inheritFrom,
            CallbackTypedefs = callbackTypedefs,
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

        // Build TypeScript parameter string
        var tsParams = nonThisParams.Select(p =>
        {
            var name = ConvertName(p.Name, NameContext.Parameter);
            var type = GetTargetType(p.Type);
            var defaultVal = p.HasExplicitDefaultValue ? FormatDefaultValue(p) : null;
            return defaultVal != null ? $"{name}: {type} = {defaultVal}" : $"{name}: {type}";
        });
        var tsParametersStr = string.Join(", ", tsParams);

        // Build native call arguments
        var nativeArgs = new List<string>();

        if (!isStaticMethod)
            nativeArgs.Add("this._ptr");

        nativeArgs.AddRange(nonThisParams.Select(p =>
            GetInteropValue(p, ConvertName(p.Name, NameContext.Parameter))));
        var nativeCallArgsStr = string.Join(", ", nativeArgs);

        // Build C signature for koffi.func()
        var cSignature = BuildCSignature(method, targetType);

        // Determine return type and class name
        var tsReturnType = GetReturnTypeName(method);
        var returnClassName = tsReturnType != "void" ? tsReturnType : null;

        // Extract unique ID from native entry point (the hash at the end)
        var nativeEntryPoint = method.GetNativeMethodName(className);
        var uniqueId = nativeEntryPoint.ExtractNativeMethodHash();

        // Use disambiguated name for overloads (makes them private with _prefix)
        var methodName = overloadInfo.IsOverload
            ? ConvertName(overloadInfo.DisambiguatedName, NameContext.Method)
            : ConvertName(method.Name, NameContext.Method);

        return new
        {
            TsMethodName = methodName,
            NativeMethodName = nativeEntryPoint,
            TsParameters = tsParametersStr,
            TsReturnType = tsReturnType,
            ReturnClassName = returnClassName,
            CSignature = cSignature,
            NativeCallArgs = nativeCallArgsStr,
            UniqueId = uniqueId,
            DeprecationMessage = method.TryGetDeprecationMessage(),
            IsStaticMethod = isStaticMethod,
            Callbacks = method.GetCallbackParameters().Select(c => new
            {
                ParameterName = ConvertName(c.Name, NameContext.Parameter),
                ArgumentTypeName = c.GetCallbackArgumentTypeName()
            }).ToList(),
            IsOverload = overloadInfo.IsOverload,
            OriginalName = ConvertName(method.Name, NameContext.Method)
        };
    }

    private string GetReturnTypeName(IMethodSymbol method)
    {
        var kind = method.ReturnType.GetInteropTypeKind();

        if (kind == InteropTypeKind.Void)
            return "void";

        if (kind == InteropTypeKind.TypeParameter)
            return _currentClassName;

        return GetTargetType(method.ReturnType);
    }

    private string BuildCSignature(IMethodSymbol method, INamedTypeSymbol targetType)
    {
        var returnType = GetKoffiType(method.ReturnType);
        var className = targetType.GetGeneratedClassName();
        var methodName = method.GetNativeMethodName(className);
        var isStaticMethod = method.IsStatic && !method.IsExtensionMethod;

        // Build parameters: first is void* target for instance methods
        var parameters = new List<string>();

        if (!isStaticMethod)
            parameters.Add("void* target");

        parameters.AddRange(method.GetNonThisParameters().Select(p =>
        {
            var koffiType = GetKoffiType(p.Type);
            return $"{koffiType} {p.Name}";
        }));

        return $"{returnType} {methodName}({string.Join(", ", parameters)})";
    }
}
