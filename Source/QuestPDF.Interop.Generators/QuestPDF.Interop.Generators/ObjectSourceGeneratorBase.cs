using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal abstract class ObjectSourceGeneratorBase : IInteropSourceGenerator
{
    protected abstract IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation);
    
    public string GenerateCSharpCode(Compilation compilation)
    {
        return ScribanTemplateLoader
            .LoadTemplate("NativeInteropMethod.cs")
            .Render(new
            {
                Methods = GetTargetMethods(compilation).Select(MapMethod)
            });
        
        static object MapMethod(IMethodSymbol method)
        {
            var parameters = method.Parameters.Select(GetMethodParameter);
            
            if (!method.IsExtensionMethod)
                parameters = parameters.Prepend("IntPtr target");
            
            return new NativeInteropMethodTemplateModel
            {
                NativeName = method.GetNativeMethodName(),
                ManagedName = method.GetManagedMethodName(),
                ApiName = method.Name,
                MethodParameters = parameters,
                TargetObjectType = method.IsExtensionMethod ? method.Parameters.First().Type.Name : method.ContainingType.Name,
                TargetObjectParameterName = method.IsExtensionMethod ? method.Parameters.First().Name : "target",
                TargetMethodParameters = method.Parameters.Skip(method.IsExtensionMethod ? 1 : 0).Select(GetTargetMethodParameter),
                ReturnType = method.ReturnType.GetInteropResultType(),
                ShouldFreeTarget = !method.IsExtensionMethod
            };
        }

        static string GetMethodParameter(IParameterSymbol parameterSymbol)
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
        
        static string GetTargetMethodParameter(IParameterSymbol parameterSymbol)
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
                return $"x => {{ var boxed = BoxHandle(x); var result = {parameterSymbol.Name}(boxed); FreeHandle(boxed); return UnboxHandle<{resultType.Name}>(result); }}";
            }
            
            if (parameterSymbol.Type.SpecialType == SpecialType.System_String)
            {
                return $"Marshal.PtrToStringAuto({parameterSymbol.Name})";
            }

            return parameterSymbol.Name;
        }
    }
    
    public string GeneratePythonCode(Compilation compilation)
    {
        var headers = GetTargetMethods(compilation)
            .Select(x => x.GetCHeaderDefinition());

        return ScribanTemplateLoader
            .LoadTemplate("Container.py")
            .Render(new
            {
                Headers = headers,
                Methods = GetTargetMethods(compilation).Select(MapMethod)
            });
        
        static object MapMethod(IMethodSymbol method)
        {
            return new
            {
                PythonMethodName = method.Name.ToSnakeCase(),
                PythonMethodParameters = method.Parameters.Skip(1).Select(GetMethodParameter).Prepend("self"),
                
                InteropMethodName = method.GetNativeMethodName(),
                InteropMethodParameters = method.Parameters.Skip(1).Select(GetInteropMethodParameter).Prepend("self.container_pointer"),
                PythonMethodReturnType = "not_used",
                
                DeprecationMessage = method.TryGetDeprecationMessage(),
                
                Callbacks = GetCallbacks(method)
            };
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
                    
                    result += $" = {parameterSymbol.Type.Name}.{enumValueName.ToPythonEnumMemberName()}";
                }
                else
                {
                    result += $" = {parameterSymbol.ExplicitDefaultValue ?? "None"}";
                }
            }
            
            return result;
        }
        
        static string GetInteropMethodParameter(IParameterSymbol parameterSymbol)
        {
            var parameterName = parameterSymbol.Name.ToSnakeCase();
            
            if (parameterSymbol.Type.TypeKind == TypeKind.Enum)
                return $"{parameterName}.value";
            
            return parameterName;
        }

        static IEnumerable<object> GetCallbacks(IMethodSymbol methodSymbol)
        {
            foreach (var parameterSymbol in methodSymbol.Parameters.Where(x => x.Type.IsAction()))
            {
                yield break;
            }
        }
    }
}