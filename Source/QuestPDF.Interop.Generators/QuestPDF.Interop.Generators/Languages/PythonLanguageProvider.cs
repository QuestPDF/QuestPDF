using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

internal class PythonLanguageProvider : LanguageProviderBase
{
    protected override string SelfParameterName => "self";
    protected override string SelfPointerExpression => "self.target_pointer";
    protected override string OverloadMethodNamePrefix => "_";

    public override string ConvertName(string csharpName, NameContext context)
    {
        return context switch
        {
            NameContext.Class => csharpName,
            _ => csharpName.ToSnakeCase()
        };
    }

    protected override string GetTargetType(ITypeSymbol type)
    {
        var kind = type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Void => "None",
            InteropTypeKind.Boolean => "bool",
            InteropTypeKind.Int => "int",
            InteropTypeKind.Float => "float",
            InteropTypeKind.String => "str",
            InteropTypeKind.ByteArray => "bytes",
            InteropTypeKind.Color => "Color",
            InteropTypeKind.TypeParameter or InteropTypeKind.Unknown => "Any",
            InteropTypeKind.Action or InteropTypeKind.Func => FormatCallableType((INamedTypeSymbol)type, isFunc: kind == InteropTypeKind.Func),
            _ => type.GetInteropTypeName()
        };
    }

    protected override string FormatDefaultValue(IParameterSymbol parameter)
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

    protected override string GetInteropValue(IParameterSymbol parameter, string variableName)
    {
        var kind = parameter.Type.GetInteropTypeKind();
        return kind switch
        {
            InteropTypeKind.Color => $"{variableName}.hex",
            InteropTypeKind.Enum => $"{variableName}.value",
            InteropTypeKind.String => $"questpdf_ffi.new(\"char[]\", {variableName}.encode(\"utf-8\"))",
            InteropTypeKind.Action or InteropTypeKind.Func => $"_internal_{variableName}_handler",
            _ => variableName
        };
    }

    protected override List<string> BuildHeaders(IReadOnlyList<IMethodSymbol> methods, string className)
    {
        return methods.Select(m => m.GetCHeaderDefinition(className)).ToList();
    }

    protected override string GetReturnConversionMethod(IMethodSymbol method, string className)
    {
        var kind = method.ReturnType.GetInteropTypeKind();

        if (kind == InteropTypeKind.Void)
            return null;

        if (kind == InteropTypeKind.String)
            return "decode_text_as_utf_8";

        if (kind == InteropTypeKind.ByteArray)
            return "questpdf_ffi.string";

        return GetReturnTypeName(method, className);
    }

    private string FormatCallableType(INamedTypeSymbol type, bool isFunc)
    {
        if (type.TypeArguments.Length == 0)
            return "Callable[[], Any]";

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

    private static string GetTargetTypeForCallable(ITypeSymbol type)
    {
        return type.GetInteropTypeKind() switch
        {
            InteropTypeKind.ByteArray => "bytes",
            InteropTypeKind.String => "str",
            _ => type.GetInteropTypeName()
        };
    }
}
