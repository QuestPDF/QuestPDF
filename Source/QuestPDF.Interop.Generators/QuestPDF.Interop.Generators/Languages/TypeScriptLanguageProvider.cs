using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

internal class TypeScriptLanguageProvider : LanguageProviderBase
{
    protected override string SelfParameterName => null;
    protected override string SelfPointerExpression => "this._ptr";

    protected override string ConvertName(string csharpName, NameContext context)
    {
        return context switch
        {
            NameContext.Method or NameContext.Parameter or NameContext.Property => csharpName.ToCamelCase(),
            NameContext.Constant => csharpName.ToSnakeCase().ToUpperInvariant(),
            _ => csharpName
        };
    }

    protected override string GetTargetType(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Void => "void",
            InteropTypeKind.Boolean => "boolean",
            InteropTypeKind.Int or InteropTypeKind.Float => "number",
            InteropTypeKind.String => "string",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Enum or InteropTypeKind.Class or InteropTypeKind.Interface => type.GetInteropTypeName(),
            InteropTypeKind.Action or InteropTypeKind.Func => FormatFunctionType((INamedTypeSymbol)type, isFunc: kind == InteropTypeKind.Func),
            _ => "unknown"
        };
    }

    protected override string FormatDefaultValue(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        if (parameter.ExplicitDefaultValue == null)
            return "undefined";

        var kind = parameter.Type.GetInteropTypeKind();

        if (kind == InteropTypeKind.Enum && parameter.GetDefaultEnumMemberName() is { } enumMember)
            return $"{parameter.Type.Name}.{enumMember}";

        if (parameter.ExplicitDefaultValue is bool boolValue)
            return boolValue ? "true" : "false";

        if (parameter.ExplicitDefaultValue is string stringValue)
            return $"'{stringValue}'";

        return parameter.ExplicitDefaultValue?.ToString() ?? "undefined";
    }

    protected override string GetInteropValue(IParameterSymbol parameter, string variableName)
    {
        var kind = parameter.Type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Color => $"{variableName}.hex",
            InteropTypeKind.Action or InteropTypeKind.Func => $"{variableName}Cb",
            InteropTypeKind.Class or InteropTypeKind.Interface => $"{variableName}.pointer",
            _ => variableName
        };
    }

    protected override string BuildNativeSignature(IMethodSymbol method, INamedTypeSymbol targetType)
    {
        var returnType = GetKoffiType(method.ReturnType);
        var className = targetType.GetGeneratedClassName();
        var methodName = method.GetNativeMethodName(className);
        var isStaticMethod = method.IsStatic && !method.IsExtensionMethod;

        var parameters = new List<string>();

        if (!isStaticMethod)
            parameters.Add("void* target");

        parameters.AddRange(method.GetNonThisParameters().Select(p =>
            $"{GetKoffiType(p.Type)} {p.Name}"));

        return $"{returnType} {methodName}({string.Join(", ", parameters)})";
    }

    private string GetKoffiType(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Void => "void",
            InteropTypeKind.Boolean => "uint8_t",
            InteropTypeKind.Int or InteropTypeKind.Float => GetKoffiNumericType(type),
            InteropTypeKind.String => "str16",
            InteropTypeKind.Enum => "int32_t",
            InteropTypeKind.Color => "uint32_t",
            InteropTypeKind.Action or InteropTypeKind.Func => "void*",
            InteropTypeKind.Class or InteropTypeKind.Interface or InteropTypeKind.TypeParameter => "void*",
            _ => "void*"
        };
    }

    private static string GetKoffiNumericType(ITypeSymbol type) => type.SpecialType switch
    {
        SpecialType.System_Int64 => "int64_t",
        SpecialType.System_UInt64 => "uint64_t",
        SpecialType.System_Int16 => "int16_t",
        SpecialType.System_UInt16 => "uint16_t",
        SpecialType.System_Byte => "uint8_t",
        SpecialType.System_SByte => "int8_t",
        SpecialType.System_UInt32 => "uint32_t",
        SpecialType.System_Double => "double",
        SpecialType.System_Single => "float",
        _ => "int32_t"
    };

    private string FormatFunctionType(INamedTypeSymbol type, bool isFunc)
    {
        if (type.TypeArguments.Length == 0)
            return isFunc ? "() => unknown" : "() => void";
        
        var args = type
            .TypeArguments
            .SkipLast(isFunc ? 1 : 0)
            .Select((t, i) => $"arg{i}: {GetTargetTypeForCallback(t)}");
        
        var returnType = isFunc ? GetTargetTypeForCallback(type.TypeArguments.Last()) : "void";
        
        return $"({string.Join(", ", args)}) => {returnType}";
    }
}
