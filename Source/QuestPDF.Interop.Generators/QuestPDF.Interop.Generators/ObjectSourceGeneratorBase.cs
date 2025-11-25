using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal abstract class ObjectSourceGeneratorBase : IInteropSourceGenerator
{
    protected abstract IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation);
    protected abstract INamedTypeSymbol GetTargetType(Compilation compilation);

    private string GetTargetClassName(Compilation compilation)
    {
        var targetType = GetTargetType(compilation);
        
        if (targetType.TypeKind == TypeKind.Interface)
            return targetType.Name.TrimStart('I');
        
        return targetType.Name;
    }

    public string GenerateCSharpCode(Compilation compilation)
    {
        return FluidTemplateLoader
            .RenderTemplate("NativeInteropMethod.cs", new
            {
                Methods = GetTargetMethods(compilation).Select(MapMethod)
            });
        
        object MapMethod(IMethodSymbol method)
        {
            var parameters = method.Parameters.Select(GetMethodParameter);
            
            if (!method.IsExtensionMethod)
                parameters = parameters.Prepend("IntPtr target");
            
            return new NativeInteropMethodTemplateModel
            {
                NativeName = method.GetNativeMethodName(GetTargetClassName(compilation)),
                ManagedName = method.GetManagedMethodName(GetTargetClassName(compilation)),
                ApiName = method.Name,
                MethodParameters = parameters,
                TargetObjectType = GetTargetType(compilation).Name,
                TargetObjectParameterName = method.IsExtensionMethod ? method.Parameters.First().Name : "target",
                TargetMethodParameters = method.Parameters.Skip(method.IsExtensionMethod ? 1 : 0).Select(GetTargetMethodParameter),
                ReturnType = method.GetInteropResultType(),
                ShouldFreeTarget = method.IsExtensionMethod
            };
        }

        string GetMethodParameter(IParameterSymbol parameterSymbol)
        {
            if (parameterSymbol.Type.IsAction() && parameterSymbol.Type is INamedTypeSymbol actionSymbol)
            {
                var genericTypes = actionSymbol
                    .TypeArguments
                    .Select(x => x.GetInteropMethodParameterType())
                    .Append("void");
                
                var genericTypesString = string.Join(", ", genericTypes);
                return $"delegate* unmanaged[Cdecl]<{genericTypesString}> {parameterSymbol.Name}";
            }
            
            if (parameterSymbol.Type.IsFunc() && parameterSymbol.Type is INamedTypeSymbol funcSymbol)
            {
                var genericTypes = funcSymbol
                    .TypeArguments
                    .Select(x => x.GetInteropMethodParameterType());
                
                var genericTypesString = string.Join(", ", genericTypes);
                return $"delegate* unmanaged[Cdecl]<{genericTypesString}> {parameterSymbol.Name}";
            }

            if (parameterSymbol.Type.SpecialType == SpecialType.System_String)
            {
                return $"IntPtr {parameterSymbol.Name}";
            }
        
            return $"{parameterSymbol.Type.GetInteropMethodParameterType()} {parameterSymbol.Name}";
        }
        
        string GetTargetMethodParameter(IParameterSymbol parameterSymbol)
        {
            if (parameterSymbol.Type.TypeKind == TypeKind.Enum)
                return $"({parameterSymbol.Type.ToDisplayString()}){parameterSymbol.Name}";
            
            if (parameterSymbol.Type.Name == "QuestPDF.Infrastructure.Color")
                return $"(QuestPDF.Infrastructure.Color){parameterSymbol.Name}";
            
            if (parameterSymbol.Type.IsAction())
                return $"x => {{ var boxed = BoxHandle(x); {parameterSymbol.Name}(boxed); FreeHandle(boxed); }}";

            if (parameterSymbol.Type.IsFunc())
            {
                var resultType = ((INamedTypeSymbol)parameterSymbol.Type).TypeArguments.Last();
                return $"x => {{ var boxed = BoxHandle(x); var result = {parameterSymbol.Name}(boxed); FreeHandle(boxed); return UnboxHandle<{GetTargetType(compilation).Name}>(result); }}";
            }
            
            if (parameterSymbol.Type.SpecialType == SpecialType.System_String)
            {
                return $"Marshal.PtrToStringUni({parameterSymbol.Name})";
            }

            return parameterSymbol.Name;
        }
    }
    
    public string GeneratePythonCode(Compilation compilation)
    {
        var targetMethods = GetTargetMethods(compilation).ToList();

        var callbackTypedefs = targetMethods.GetCallbackTypedefs()
            .Select(t => t.TypedefDefinition);

        var headers = targetMethods
            .Select(x => x.GetCHeaderDefinition(GetTargetClassName(compilation)));

        return FluidTemplateLoader
            .RenderTemplate("Object.py", new
            {
                CallbackTypedefs = callbackTypedefs,
                Headers = headers,
                ClassName = GetTargetClassName(compilation),
                Methods = targetMethods.Select(MapMethod)
            });
        
        object MapMethod(IMethodSymbol method)
        {
            return new
            {
                PythonMethodName = method.Name.ToSnakeCase(),
                PythonMethodParameters = method.Parameters.Skip(method.IsExtensionMethod ? 1 : 0).Select(GetMethodParameter).Prepend("self"),
                
                InteropMethodName = method.GetNativeMethodName(GetTargetClassName(compilation)),
                InteropMethodParameters = method.Parameters.Skip(method.IsExtensionMethod ? 1 : 0).Select(GetInteropMethodParameter).Prepend("self.target_pointer"),
                PythonMethodReturnType = GetReturnType(),
                
                DeprecationMessage = method.TryGetDeprecationMessage(),
                
                Callbacks = GetCallbacks(method)
            };

            string GetReturnType()
            {
                if (method.ReturnType.SpecialType == SpecialType.System_Void)
                    return null;
                
                if (method.ReturnType.TypeKind == TypeKind.TypeParameter)
                    return GetTargetClassName(compilation);
                
                if (method.ReturnType.TypeKind == TypeKind.Interface)
                    return method.ReturnType.Name.TrimStart('I');
                
                return method.ReturnType.Name;
            }
        }

        static string GetMethodParameter(IParameterSymbol parameterSymbol)
        {
            var result = $"{parameterSymbol.Name.ToSnakeCase()}: {parameterSymbol.Type.GetPythonParameterType()}";
            
            if (parameterSymbol.HasExplicitDefaultValue)
            {
                if (parameterSymbol.ExplicitDefaultValue == null)
                {
                    result += " = None";
                }
                else if (parameterSymbol.Type.TypeKind == TypeKind.Enum)
                {
                    var enumValueName = parameterSymbol.Type
                        .GetMembers()
                        .OfType<IFieldSymbol>()
                        .First(x => x.HasConstantValue && x.ConstantValue.Equals(parameterSymbol.ExplicitDefaultValue))
                        .Name;
                    
                    result += $" = {parameterSymbol.Type.Name}.{enumValueName.ToSnakeCase()}";
                }
                else
                {
                    var defaultValue = parameterSymbol.ExplicitDefaultValue;
                    
                    if (defaultValue is string)
                        defaultValue = $"'{defaultValue}'";
                    
                    result += $" = {defaultValue ?? "None"}";
                }
            }
            
            return result;
        }
        
        static string GetInteropMethodParameter(IParameterSymbol parameterSymbol)
        {
            var parameterName = parameterSymbol.Name.ToSnakeCase();

            if (parameterSymbol.Type.TypeKind == TypeKind.Enum)
                return $"{parameterName}.value";
            
            if (parameterSymbol.Type.SpecialType == SpecialType.System_String)
                return $"questpdf_ffi.new(\"char[]\", {parameterName}.encode(\"utf-16\"))";

            if (parameterSymbol.Type.IsAction())
                return $"_internal_{parameterName}_handler";

            return parameterName;
        }

        static IEnumerable<object> GetCallbacks(IMethodSymbol methodSymbol)
        {
            foreach (var parameterSymbol in methodSymbol.Parameters.Skip(1).Where(x => x.Type.IsAction()))
            {
                var actionType = (INamedTypeSymbol)parameterSymbol.Type;
                var callbackArgument = actionType.TypeArguments.FirstOrDefault();

                if (callbackArgument == null)
                    continue;

                var pythonParameterName = parameterSymbol.Name.ToSnakeCase();
                var callbackArgumentTypeName = callbackArgument.TypeKind == TypeKind.Interface
                    ? callbackArgument.Name.TrimStart('I')
                    : callbackArgument.Name;

                yield return new
                {
                    PythonParameterName = pythonParameterName,
                    CallbackArgumentTypeName = callbackArgumentTypeName,
                    InternalCallbackName = $"_internal_{pythonParameterName}_handler"
                };
            }
        }
    }
    
    public string GenerateJavaCode(Compilation compilation)
    {
        return string.Empty;
    }
    
    public string GenerateTypeScriptCode(Compilation compilation)
    {
        return string.Empty;
    }
}