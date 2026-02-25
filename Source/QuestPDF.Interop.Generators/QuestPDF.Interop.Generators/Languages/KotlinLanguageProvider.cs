using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

internal class KotlinLanguageProvider : LanguageProviderBase
{
    protected override string SelfParameterName => null;
    protected override string SelfPointerExpression => "pointer";

    // "Unit" in C# is a measurement type, but in Kotlin "Unit" means void — rename to avoid conflict
    private static readonly Dictionary<string, string> KotlinReservedWordMappings = new()
    {
        ["Unit"] = "LengthUnit"
    };

    public static string ConvertTypeName(string csharpTypeName)
    {
        return KotlinReservedWordMappings.TryGetValue(csharpTypeName, out var kotlinName)
            ? kotlinName
            : csharpTypeName;
    }

    public override string ConvertName(string csharpName, NameContext context)
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
            InteropTypeKind.Void => "Unit",
            InteropTypeKind.Boolean => "Boolean",
            InteropTypeKind.Int or InteropTypeKind.Float => GetNumericType(type),
            InteropTypeKind.String => "String",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.Enum => ConvertTypeName(type.Name),
            InteropTypeKind.Class or InteropTypeKind.Interface => type.GetInteropTypeName(),
            InteropTypeKind.Action => FormatFunctionType((INamedTypeSymbol)type, isFunc: false),
            InteropTypeKind.Func => FormatFunctionType((INamedTypeSymbol)type, isFunc: true),
            _ => "Any"
        };
    }

    protected override string FormatDefaultValue(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        if (parameter.ExplicitDefaultValue == null)
            return "null";

        var kind = parameter.Type.GetInteropTypeKind();

        if (kind == InteropTypeKind.Enum && parameter.GetDefaultEnumMemberName() != null)
            return $"{ConvertTypeName(parameter.Type.Name)}.{parameter.GetDefaultEnumMemberName()}";

        if (parameter.ExplicitDefaultValue is bool boolValue)
            return boolValue ? "true" : "false";

        if (parameter.ExplicitDefaultValue is string stringValue)
            return $"\"{stringValue}\"";

        if (parameter.Type.SpecialType == SpecialType.System_Single)
            return $"{parameter.ExplicitDefaultValue}f";

        return parameter.ExplicitDefaultValue?.ToString() ?? "null";
    }

    protected override string GetInteropValue(IParameterSymbol parameter, string variableName)
    {
        var kind = parameter.Type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Enum => $"{variableName}.value",
            InteropTypeKind.Boolean => $"(if ({variableName}) 1.toByte() else 0.toByte())",
            InteropTypeKind.Color => $"{variableName}.hex.toInt()",
            InteropTypeKind.Action or InteropTypeKind.Func => $"{variableName}Callback",
            InteropTypeKind.Class or InteropTypeKind.Interface => $"{variableName}.pointer",
            _ => variableName
        };
    }

    protected override string BuildNativeSignature(IMethodSymbol method, INamedTypeSymbol targetType)
    {
        var returnType = GetJnaType(method.ReturnType);
        var className = targetType.GetGeneratedClassName();
        var methodName = method.GetNativeMethodName(className);
        var isStaticMethod = method.IsStatic && !method.IsExtensionMethod;

        var parameters = new List<string>();

        if (!isStaticMethod)
            parameters.Add("target: Pointer");

        parameters.AddRange(method.GetNonThisParameters().Select(p =>
            $"{p.Name}: {GetJnaType(p.Type)}"));

        return $"fun {methodName}({string.Join(", ", parameters)}): {returnType}";
    }

    protected override string GetNativeReturnType(IMethodSymbol method)
    {
        var kind = method.ReturnType.GetInteropTypeKind();
        return kind == InteropTypeKind.Void ? "Unit" : GetJnaType(method.ReturnType);
    }

    protected override List<object> BuildParameterDetails(IReadOnlyList<IParameterSymbol> parameters)
    {
        return parameters.Select(p => (object)new
        {
            Name = p.Name,
            NativeType = GetJnaType(p.Type)
        }).ToList();
    }

    protected override List<object> BuildCallbackInterfaces(IReadOnlyList<IMethodSymbol> methods)
    {
        return methods
            .SelectMany(m => m.GetCallbackParameters())
            .Select(c => new
            {
                InterfaceName = $"{c.GetCallbackArgumentTypeName()}Callback",
                ArgumentTypeName = c.GetCallbackArgumentTypeName()
            })
            .DistinctBy(c => c.InterfaceName)
            .Select(c => (object)c)
            .ToList();
    }

    protected override List<string> BuildHeaders(IReadOnlyList<IMethodSymbol> methods, string className)
    {
        return methods.Select(m => m.GetCHeaderDefinition(className)).ToList();
    }

    private string GetJnaType(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Void => "Unit",
            InteropTypeKind.Boolean => "Byte",
            InteropTypeKind.Int or InteropTypeKind.Float => GetNumericType(type),
            InteropTypeKind.String => "String",
            InteropTypeKind.Enum => "Int",
            InteropTypeKind.Color => "Int",
            InteropTypeKind.Action or InteropTypeKind.Func => "Callback",
            InteropTypeKind.Class or InteropTypeKind.Interface or InteropTypeKind.TypeParameter => "Pointer",
            _ => "Pointer"
        };
    }

    private static string GetNumericType(ITypeSymbol type) => type.SpecialType switch
    {
        SpecialType.System_Int64 or SpecialType.System_UInt64 => "Long",
        SpecialType.System_Int16 or SpecialType.System_UInt16 => "Short",
        SpecialType.System_Byte or SpecialType.System_SByte => "Byte",
        SpecialType.System_Double => "Double",
        SpecialType.System_Single => "Float",
        _ => "Int"
    };

    private string FormatFunctionType(INamedTypeSymbol type, bool isFunc)
    {
        if (type.TypeArguments.Length == 0)
            return isFunc ? "() -> Any" : "() -> Unit";

        if (isFunc)
        {
            var args = type.TypeArguments.SkipLast(1).Select(GetTargetTypeForCallback);
            var returnType = GetTargetTypeForCallback(type.TypeArguments.Last());
            return $"({string.Join(", ", args)}) -> {returnType}";
        }
        else
        {
            var args = type.TypeArguments.Select(GetTargetTypeForCallback);
            return $"({string.Join(", ", args)}) -> Unit";
        }
    }
}
