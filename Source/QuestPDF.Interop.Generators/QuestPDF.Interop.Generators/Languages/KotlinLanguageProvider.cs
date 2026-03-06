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
        return KotlinReservedWordMappings.GetValueOrDefault(csharpTypeName, csharpTypeName);
    }

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
        
        var result = kind switch
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
            InteropTypeKind.ImageSize => "ImageSize",
            _ => "Any"
        };

        if (type.NullableAnnotation == NullableAnnotation.Annotated)
            return $"{result}?";
        
        return result;
    }

    protected override string FormatDefaultValue(IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue)
            return null;

        if (parameter.ExplicitDefaultValue == null)
            return "null";

        var kind = parameter.Type.GetInteropTypeKind();

        if (kind == InteropTypeKind.Enum && parameter.GetDefaultEnumMemberName() is { } enumMember)
            return $"{ConvertTypeName(parameter.Type.Name)}.{enumMember}";

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
        var typeName = parameter.Type.ToDisplayString();

        if (typeName is "QuestPDF.Helpers.PageSize" or "QuestPDF.Helpers.Size" or "QuestPDF.Infrastructure.ImageSize")
        {
            return $"{variableName}.width, {variableName}.height";
        }
        
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

        parameters.AddRange(
            method.GetNonThisParameters().Select(p => $"{p.Name}: {GetJnaType(p.Type)}"));

        return $"fun {methodName}({string.Join(", ", parameters)}): {returnType}";
    }

    protected override string GetNativeReturnType(IMethodSymbol method)
    {
        var kind = method.ReturnType.GetInteropTypeKind();
        return kind == InteropTypeKind.Void ? "Unit" : GetJnaType(method.ReturnType);
    }
    
    protected override string? GetReturnConversionMethod(IMethodSymbol method, string className)
    {
        if (method.ReturnType.ToDisplayString() == "QuestPDF.Infrastructure.Color")
            return "convert_int_to_color";

        return null;
    }

    protected override List<object> BuildParameterDetails(IReadOnlyList<IParameterSymbol> parameters)
    {
        return GetDetails().ToList();
        
        IEnumerable<object> GetDetails()
        {
            foreach (var parameterSymbol in parameters)
            {
                var typeName = parameterSymbol.Type.ToDisplayString();

                if (typeName is "QuestPDF.Helpers.PageSize" or "QuestPDF.Helpers.Size")
                {
                    yield return new
                    {
                        Name = $"{parameterSymbol.Name}_width",
                        NativeType = "Float"
                    };
                    
                    yield return new
                    {
                        Name = $"{parameterSymbol.Name}_height",
                        NativeType = "Float"
                    };
                }
                else if (typeName is "QuestPDF.Infrastructure.ImageSize")
                {
                    yield return new
                    {
                        Name = $"{parameterSymbol.Name}_width",
                        NativeType = "Int"
                    };
                    
                    yield return new
                    {
                        Name = $"{parameterSymbol.Name}_height",
                        NativeType = "Int"
                    };
                }
                else
                {
                    yield return new
                    {
                        Name = parameterSymbol.Name,
                        NativeType = GetJnaType(parameterSymbol.Type)
                    };
                }
            }
        }
    }

    protected override List<object> BuildCallbackInterfaces(IReadOnlyList<IMethodSymbol> methods)
    {
        return methods
            .SelectMany(method => method
                .GetCallbackParameters()
                .Select(c => new
                {
                    InterfaceName =
                        $"{c.GetCallbackArgumentTypeName()}Callback_{method.Name.GetDeterministicHash()}",
                    ArgumentTypeName = c.GetCallbackArgumentTypeName()
                }))
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
        
        var result = kind switch
        {
            InteropTypeKind.Void => "Unit",
            InteropTypeKind.Boolean => "Byte",
            InteropTypeKind.Int or InteropTypeKind.Float => GetNumericType(type),
            InteropTypeKind.String => "String",
            InteropTypeKind.Enum => "Int",
            InteropTypeKind.Color => "Int",
            InteropTypeKind.Action or InteropTypeKind.Func => "Callback",
            _ => "Pointer"
        };
        
        if (type.NullableAnnotation == NullableAnnotation.Annotated)
            return $"{result}?";
        
        return result;   
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

        var args = type
            .TypeArguments
            .SkipLast(isFunc ? 1 : 0)
            .Select(GetTargetTypeForCallback);

        if (!isFunc && type.TypeArguments.Length == 1)
        {
            return $"{args.First()}.() -> Unit";
        }
        
        var returnType = isFunc ? GetTargetTypeForCallback(type.TypeArguments.Last()) : "Unit";
        
        return $"({string.Join(", ", args)}) -> {returnType}";
    }
}
