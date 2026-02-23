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
            var parameters = method.Parameters.SelectMany(GetMethodParameter);
            
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
                TargetMethodParameters = method.Parameters.Skip(method.IsExtensionMethod ? 1 : 0).SelectMany(GetTargetMethodParameter),
                ReturnType = method.GetInteropResultType(),
                ResultTransformFunction = method.GetCSharpResultTransformFunction(),
                ShouldFreeTarget = method.IsExtensionMethod,
            };
        }

        IEnumerable<string> GetMethodParameter(IParameterSymbol parameterSymbol)
        {
            if (parameterSymbol.Type.IsAction() && parameterSymbol.Type is INamedTypeSymbol actionSymbol)
            {
                var genericTypes = actionSymbol
                    .TypeArguments
                    .Select(x => x.GetInteropMethodParameterType())
                    .Append("void");
                
                var genericTypesString = string.Join(", ", genericTypes);
                yield return $"delegate* unmanaged[Cdecl]<{genericTypesString}> {parameterSymbol.Name}";
            }
            else if (parameterSymbol.Type.IsFunc() && parameterSymbol.Type is INamedTypeSymbol funcSymbol)
            {
                var genericTypes = funcSymbol
                    .TypeArguments
                    .Select(x => x.GetInteropMethodParameterType());
                
                var genericTypesString = string.Join(", ", genericTypes);
                yield return $"delegate* unmanaged[Cdecl]<{genericTypesString}> {parameterSymbol.Name}";
            }
            else if (parameterSymbol.Type.SpecialType == SpecialType.System_String)
            {
                yield return $"IntPtr {parameterSymbol.Name}";
            }
            else if (parameterSymbol.Type.ToDisplayString() == "QuestPDF.Helpers.PageSize")
            {
                yield return $"float {parameterSymbol.Name}_width";
                yield return $"float {parameterSymbol.Name}_height";
            }
            else if (parameterSymbol.Type.ToDisplayString() == "QuestPDF.Helpers.Size")
            {
                yield return $"float {parameterSymbol.Name}_width";
                yield return $"float {parameterSymbol.Name}_height";
            }
            else
            {
                yield return $"{parameterSymbol.Type.GetInteropMethodParameterType()} {parameterSymbol.Name}";
            }
        }
        
        IEnumerable<string> GetTargetMethodParameter(IParameterSymbol parameterSymbol)
        {
            var imageType = typeof(QuestPDF.Infrastructure.Image);
            var svgImageType = typeof(QuestPDF.Infrastructure.SvgImage);
            
            if (parameterSymbol.Type.TypeKind == TypeKind.Enum)
            {
                yield return $"({parameterSymbol.Type.ToDisplayString()}){parameterSymbol.Name}";   
            }
            else if (parameterSymbol.Type.Name == "QuestPDF.Infrastructure.Color")
            {
                yield return $"(QuestPDF.Infrastructure.Color){parameterSymbol.Name}";
            }
            else if (parameterSymbol.Type.IsAction())
            {
                yield return $"x => {{ var boxed = BoxHandle(x); {parameterSymbol.Name}(boxed); FreeHandle(boxed); }}";   
            }
            else if (parameterSymbol.Type.IsFunc())
            {
                yield return $"x => {{ var boxed = BoxHandle(x); var result = {parameterSymbol.Name}(boxed); FreeHandle(boxed); return UnboxHandle<{GetTargetType(compilation).Name}>(result); }}";
            }
            else if (parameterSymbol.Type.SpecialType == SpecialType.System_String)
            {
                yield return $"Marshal.PtrToStringUTF8({parameterSymbol.Name})";
            }
            else if (parameterSymbol.Type.ToDisplayString() == "QuestPDF.Helpers.PageSize")
            {
                yield return $"new PageSize({parameterSymbol.Name}_width, {parameterSymbol.Name}_height)";
            }
            else if (parameterSymbol.Type.ToDisplayString() == "QuestPDF.Helpers.Size")
            {
                yield return $"new Size({parameterSymbol.Name}_width, {parameterSymbol.Name}_height)";
            }
            else if (parameterSymbol.Type.ToDisplayString() == imageType.FullName)
            {
                yield return $"UnboxHandle<{imageType.FullName}>({parameterSymbol.Name})";
            }
            else if (parameterSymbol.Type.ToDisplayString() == svgImageType.FullName)
            {
                yield return $"UnboxHandle<{svgImageType.FullName}>({parameterSymbol.Name})";
            }
            else
            {
                yield return parameterSymbol.Name;
            }
        }
    }
    
    public string GeneratePythonCode(Compilation compilation)
    {
        return GenerateCode(compilation, "Python", new PythonLanguageProvider());
    }

    public string GenerateTypeScriptCode(Compilation compilation)
    {
        return GenerateCode(compilation, "TypeScript", new TypeScriptLanguageProvider());
    }

    public string GenerateKotlinCode(Compilation compilation)
    {
        return GenerateCode(compilation, "Kotlin", new KotlinLanguageProvider());
    }

    private string GenerateCode(Compilation compilation, string prefix, ILanguageProvider languageProvider)
    {
        var classModel = BuildClassModel(compilation);
        var customDefinitions = TryLoadingCustomContent($"{prefix}.{GetTargetClassName(compilation)}.Object.Defs");
        var customInit = TryLoadingCustomContent($"{prefix}.{GetTargetClassName(compilation)}.Object.Init");
        var customClass = TryLoadingCustomContent($"{prefix}.{GetTargetClassName(compilation)}.Object.Class");
        
        var templateModel = languageProvider.BuildClassTemplateModel(classModel, customDefinitions, customInit, customClass);
        var mainCode = TemplateManager.RenderTemplate($"{prefix}.Object", templateModel);

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