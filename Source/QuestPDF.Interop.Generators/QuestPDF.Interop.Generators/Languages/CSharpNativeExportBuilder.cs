using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators.Languages;

// Unlike client-side providers (Python/TS/Kotlin) that marshal managed→native,
// this generates server-side export stubs that marshal native→managed.
internal class CSharpNativeExportBuilder
{
    private readonly string _className;
    private readonly string _targetTypeName;

    public CSharpNativeExportBuilder(INamedTypeSymbol targetType)
    {
        _className = targetType.GetGeneratedClassName();
        _targetTypeName = targetType.Name;
    }

    public object BuildTemplateModel(IEnumerable<IMethodSymbol> methods)
    {
        return new { Methods = methods.Select(BuildMethodModel) };
    }

    private object BuildMethodModel(IMethodSymbol method)
    {
        var isStaticMethod = method.IsStatic && !method.IsExtensionMethod;
        var isExtension = method.IsExtensionMethod;

        var parameters = method.Parameters.SelectMany(GetExportParameters);
        if (!isExtension && !method.IsStatic)
            parameters = parameters.Prepend("IntPtr target");

        var targetMethodParameters = method.Parameters
            .Skip(isExtension ? 1 : 0)
            .SelectMany(GetMarshalledArguments);

        return new
        {
            NativeName = method.GetNativeMethodName(_className),
            ManagedName = GetManagedMethodName(method, _className),
            ApiName = method.Name,
            MethodParameters = parameters,
            IsStaticMethod = isStaticMethod,
            TargetObjectName = isStaticMethod ? _targetTypeName : "targetObject",
            TargetObjectType = _targetTypeName,
            TargetObjectParameterName = isExtension ? method.Parameters.First().Name : "target",
            TargetMethodParameters = targetMethodParameters,
            ReturnType = GetInteropReturnType(method),
            ResultTransformFunction = GetResultTransformFunction(method),
            ShouldFreeTarget = isExtension,
        };
    }

    private static IEnumerable<string> GetExportParameters(IParameterSymbol parameter)
    {
        var kind = parameter.Type.GetInteropTypeKind();
        var typeName = parameter.Type.ToDisplayString();

        if (typeName is "QuestPDF.Helpers.PageSize" or "QuestPDF.Helpers.Size")
        {
            yield return $"float {parameter.Name}_width";
            yield return $"float {parameter.Name}_height";
            yield break;
        }

        switch (kind)
        {
            case InteropTypeKind.Action or InteropTypeKind.Func when parameter.Type is INamedTypeSymbol delegateType:
            {
                var genericTypes = delegateType.TypeArguments.Select(GetInteropParameterType);
                if (kind == InteropTypeKind.Action)
                    genericTypes = genericTypes.Append("void");
                yield return $"delegate* unmanaged[Cdecl]<{string.Join(", ", genericTypes)}> {parameter.Name}";
                break;
            }
            case InteropTypeKind.Color:
                yield return $"uint {parameter.Name}";
                break;
            case InteropTypeKind.String:
                yield return $"IntPtr {parameter.Name}";
                break;
            default:
                yield return $"{GetInteropParameterType(parameter.Type)} {parameter.Name}";
                break;
        }
    }

    private static string GetInteropParameterType(ITypeSymbol type)
    {
        return type.GetInteropTypeKind() switch
        {
            InteropTypeKind.TypeParameter or InteropTypeKind.Class or InteropTypeKind.Interface => "IntPtr",
            InteropTypeKind.Enum => "int",
            _ => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
        };
    }

    private IEnumerable<string> GetMarshalledArguments(IParameterSymbol parameter)
    {
        var kind = parameter.Type.GetInteropTypeKind();
        var name = parameter.Name;
        var typeName = parameter.Type.ToDisplayString();

        if (typeName is "QuestPDF.Helpers.PageSize" or "QuestPDF.Helpers.Size")
        {
            yield return $"new {parameter.Type.Name}({name}_width, {name}_height)";
            yield break;
        }

        yield return kind switch
        {
            InteropTypeKind.Enum => $"({parameter.Type.ToDisplayString()}){name}",
            InteropTypeKind.Color => $"(QuestPDF.Infrastructure.Color){name}",
            InteropTypeKind.Action => $"x => {{ var boxed = BoxHandle(x); {name}(boxed); FreeHandle(boxed); }}",
            InteropTypeKind.Func => $"x => {{ var boxed = BoxHandle(x); var result = {name}(boxed); FreeHandle(boxed); return UnboxHandle<{_targetTypeName}>(result); }}",
            InteropTypeKind.String => $"Marshal.PtrToStringUTF8({name})",
            InteropTypeKind.Class or InteropTypeKind.Interface => $"UnboxHandle<{parameter.Type.ToDisplayString()}>({name})",
            _ => name
        };
    }

    private static string GetInteropReturnType(IMethodSymbol method)
    {
        return method.ReturnType.GetInteropTypeKind() switch
        {
            InteropTypeKind.Color => "uint",
            InteropTypeKind.TypeParameter or InteropTypeKind.Class or InteropTypeKind.Interface or InteropTypeKind.String => "IntPtr",
            InteropTypeKind.ByteArray => "Buffer",
            _ => method.ReturnType.ToString()
        };
    }

    private static string GetResultTransformFunction(IMethodSymbol method)
    {
        return method.ReturnType.GetInteropTypeKind() switch
        {
            InteropTypeKind.ByteArray => "HandleBuffer",
            InteropTypeKind.String => "HandleText",
            InteropTypeKind.Class or InteropTypeKind.Interface or InteropTypeKind.TypeParameter => "BoxHandle",
            _ => "NoTransformation"
        };
    }

    private static string GetManagedMethodName(IMethodSymbol methodSymbol, string targetTypeName)
    {
        var hash = methodSymbol.ToDisplayString().GetDeterministicHash();
        return $"{targetTypeName}_{methodSymbol.Name}_{hash}";
    }
}
