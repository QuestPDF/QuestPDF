using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

/// <summary>
/// Language provider for TypeScript code generation using koffi.
/// </summary>
internal class TypeScriptLanguageProvider : LanguageProviderBase
{
    protected override string SelfParameterName => null;
    protected override string SelfPointerExpression => "this._ptr";

    public override string ConvertName(string csharpName, NameContext context)
    {
        return context switch
        {
            NameContext.Method => csharpName.ToCamelCase(),
            NameContext.Parameter => csharpName.ToCamelCase(),
            NameContext.Property => csharpName.ToCamelCase(),
            NameContext.Constant => csharpName.ToSnakeCase().ToUpperInvariant(),
            _ => csharpName // PascalCase for Class, EnumValue
        };
    }

    protected override string GetTargetType(ITypeSymbol type)
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
            InteropTypeKind.TypeParameter => "unknown",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Action => FormatFunctionType((INamedTypeSymbol)type, isFunc: false),
            InteropTypeKind.Func => FormatFunctionType((INamedTypeSymbol)type, isFunc: true),
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

        if (kind == InteropTypeKind.Enum && parameter.GetDefaultEnumMemberName() != null)
            return $"{parameter.Type.Name}.{parameter.GetDefaultEnumMemberName()}";

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
            InteropTypeKind.Action => $"{variableName}Cb",
            InteropTypeKind.Func => $"{variableName}Cb",
            InteropTypeKind.Class => $"{variableName}.pointer",
            InteropTypeKind.Interface => $"{variableName}.pointer",
            _ => variableName
        };
    }

    // ─── TypeScript-specific overrides ──────────────────────────────

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

    // ─── Koffi type mapping ─────────────────────────────────────────

    private string GetKoffiType(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Void => "void",
            InteropTypeKind.Boolean => "uint8_t",
            InteropTypeKind.Integer => GetKoffiIntegerType(type),
            InteropTypeKind.Float => GetKoffiFloatType(type),
            InteropTypeKind.String => "str16",
            InteropTypeKind.Enum => "int32_t",
            InteropTypeKind.Color => "uint32_t",
            InteropTypeKind.Action or InteropTypeKind.Func => "void*",
            InteropTypeKind.Class or InteropTypeKind.Interface or InteropTypeKind.TypeParameter => "void*",
            _ => "void*"
        };
    }

    private static string GetKoffiIntegerType(ITypeSymbol type)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        if (typeName.Contains("Int64") || typeName.Contains("long")) return "int64_t";
        if (typeName.Contains("UInt64") || typeName.Contains("ulong")) return "uint64_t";
        if (typeName.Contains("Int16") || typeName.Contains("short")) return "int16_t";
        if (typeName.Contains("UInt16") || typeName.Contains("ushort")) return "uint16_t";
        if (typeName.Contains("Byte")) return "uint8_t";
        if (typeName.Contains("SByte")) return "int8_t";
        if (typeName.Contains("UInt32") || typeName.Contains("uint")) return "uint32_t";
        return "int32_t";
    }

    private static string GetKoffiFloatType(ITypeSymbol type)
    {
        var typeName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        return typeName.Contains("Double") || typeName.Contains("double") ? "double" : "float";
    }

    // ─── TypeScript function type formatting ────────────────────────

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
}
