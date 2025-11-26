using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using QuestPDF.Interop.Generators.Languages;
using QuestPDF.Interop.Generators.Models;

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

    /// <summary>
    /// Builds a language-agnostic class model from the compilation.
    /// This model can be reused across all target languages.
    /// </summary>
    protected InteropClassModel BuildClassModel(Compilation compilation)
    {
        var targetType = GetTargetType(compilation);
        var methods = GetTargetMethods(compilation);
        return InteropModelBuilder.BuildClassModel(targetType, methods);
    }

    public string GenerateCSharpCode(Compilation compilation)
    {
        return TemplateManager
            .RenderTemplate("CSharp.NativeInteropMethod", new
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
        var classModel = BuildClassModel(compilation);
        var provider = LanguageProviderRegistry.Python;
        var templateModel = provider.BuildClassTemplateModel(classModel);

        return TemplateManager.RenderTemplate(provider.ObjectTemplateName, templateModel);
    }

    public string GenerateJavaCode(Compilation compilation)
    {
        var classModel = BuildClassModel(compilation);
        var provider = LanguageProviderRegistry.Java;
        var templateModel = provider.BuildClassTemplateModel(classModel);

        return TemplateManager.RenderTemplate(provider.ObjectTemplateName, templateModel);
    }

    public string GenerateTypeScriptCode(Compilation compilation)
    {
        var classModel = BuildClassModel(compilation);
        var provider = LanguageProviderRegistry.TypeScript;
        var templateModel = provider.BuildClassTemplateModel(classModel);

        return TemplateManager.RenderTemplate(provider.ObjectTemplateName, templateModel);
    }
}