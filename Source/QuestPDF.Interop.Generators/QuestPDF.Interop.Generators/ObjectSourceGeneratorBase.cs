using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using QuestPDF.Interop.Generators.Languages;
using QuestPDF.Interop.Generators.Models;

namespace QuestPDF.Interop.Generators;

internal abstract class ObjectSourceGeneratorBase : IInteropSourceGenerator
{
    public string? InheritFrom { get; set; }
    
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
        var result = InteropModelBuilder.BuildClassModel(targetType, methods);
        result.InheritFrom = InheritFrom;
        return result;
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
            
            if (!method.IsExtensionMethod && !method.IsStatic)
                parameters = parameters.Prepend("IntPtr target");
            
            return new NativeInteropMethodTemplateModel
            {
                NativeName = method.GetNativeMethodName(GetTargetClassName(compilation)),
                ManagedName = method.GetManagedMethodName(GetTargetClassName(compilation)),
                ApiName = method.Name,
                MethodParameters = parameters,
                IsStaticMethod = (method.IsStatic && !method.IsExtensionMethod),
                TargetObjectName = (method.IsStatic && !method.IsExtensionMethod) ? GetTargetType(compilation).Name : "targetObject",
                TargetObjectType = GetTargetType(compilation).Name,
                TargetObjectParameterName = method.IsExtensionMethod ? method.Parameters.First().Name : "target",
                TargetMethodParameters = method.Parameters.Skip(method.IsExtensionMethod ? 1 : 0).Select(GetTargetMethodParameter),
                ReturnType = method.GetInteropResultType(),
                ResultTransformFunction = method.GetCSharpResultTransformFunction(),
                ShouldFreeTarget = method.IsExtensionMethod,
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
                return $"Marshal.PtrToStringUTF8({parameterSymbol.Name})";
            }

            return parameterSymbol.Name;
        }
    }
    
    public string GeneratePythonCode(Compilation compilation)
    {
        var classModel = BuildClassModel(compilation);
        var provider = LanguageProviderRegistry.Python;
        
        var customInit = TryLoadingCustomContent($"Python.{GetTargetClassName(compilation)}.Object.Init");
        var customClass = TryLoadingCustomContent($"Python.{GetTargetClassName(compilation)}.Object.Class");
        
        var templateModel = provider.BuildClassTemplateModel(classModel, customInit, customClass);
        var mainCode = TemplateManager.RenderTemplate(provider.ObjectTemplateName, templateModel);

        return mainCode;
    }

    public string GenerateTypeScriptCode(Compilation compilation)
    {
        var classModel = BuildClassModel(compilation);
        var provider = LanguageProviderRegistry.TypeScript;
        
        var customInit = TryLoadingCustomContent($"TypeScript.{GetTargetClassName(compilation)}.Object.Init");
        var customClass = TryLoadingCustomContent($"TypeScript.{GetTargetClassName(compilation)}.Object.Class");
        
        var templateModel = provider.BuildClassTemplateModel(classModel, customInit, customClass);
        var mainCode = TemplateManager.RenderTemplate(provider.ObjectTemplateName, templateModel);

        return mainCode;
    }

    public string GenerateKotlinCode(Compilation compilation)
    {
        var classModel = BuildClassModel(compilation);
        var provider = LanguageProviderRegistry.Kotlin;
        
        var customInit = TryLoadingCustomContent($"Kotlin.{GetTargetClassName(compilation)}.Object.Init");
        var customClass = TryLoadingCustomContent($"Kotlin.{GetTargetClassName(compilation)}.Object.Class");
        
        var templateModel = provider.BuildClassTemplateModel(classModel, customInit, customClass);
        var mainCode = TemplateManager.RenderTemplate(provider.ObjectTemplateName, templateModel);

        return mainCode;
    }
    
    private string TryLoadingCustomContent(string id)
    {
        using var stream = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream($"QuestPDF.Interop.Generators.Templates.{id}.liquid");
        
        if (stream == null)
            return string.Empty;

        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }
}