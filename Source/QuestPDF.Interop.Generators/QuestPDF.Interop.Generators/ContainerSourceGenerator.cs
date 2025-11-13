using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal class ContainerSourceGenerator(IEnumerable<string> TargetNamespaces) : IInteropSourceGenerator
{
    class MethodTemplateModel
    {
        public string NativeName { get; set; }
        public string ManagedName { get; set; }
        public string ApiName { get; set; }
        public IEnumerable<string> MethodParameters { get; set; }
        public string TargetObjectType { get; set; }
        public string TargetObjectParameterName { get; set; }
        public IEnumerable<string> TargetMethodParameters { get; set; }
        public string? ReturnType { get; set; }
    }

    private IEnumerable<IMethodSymbol> GetTargetMethods(Compilation compilation)
    {
        return TargetNamespaces
            .Select(compilation.GetTypeByMetadataName)
            .SelectMany(x => x.GetMembers())
            .OfType<IMethodSymbol>()
            .Where(m => m.DeclaredAccessibility == Accessibility.Public && m.IsStatic && m.IsExtensionMethod);
    }
    
    public string GenerateCSharpCode(Compilation compilation)
    {
        return ScribanTemplateLoader
            .LoadTemplate("Container.cs")
            .Render(new
            {
                Methods = GetTargetMethods(compilation).Select(MapMethod)
            });
        
        static object MapMethod(IMethodSymbol method)
        {
            return new MethodTemplateModel
            {
                NativeName = method.GetNativeMethodName(),
                ManagedName = method.GetManagedMethodName(),
                ApiName = method.Name,
                MethodParameters = method.Parameters.Select(GetMethodParameter),
                TargetObjectType = method.Parameters.First().Type.Name,
                TargetObjectParameterName = method.Parameters.First().Name,
                TargetMethodParameters = method.Parameters.Skip(1).Select(GetTargetMethodParameter),
                ReturnType = method.ReturnType.GetInteropResultType()
            };
        }

        static string GetMethodParameter(IParameterSymbol parameterSymbol)
        {
            return $"{parameterSymbol.Type.GetInteropMethodParameterType()} {parameterSymbol.Name}";
        }
        
        static string GetTargetMethodParameter(IParameterSymbol parameterSymbol)
        {
            if (parameterSymbol.Type.TypeKind == TypeKind.Enum)
                return $"({parameterSymbol.Type.ToDisplayString()}){parameterSymbol.Name}";
            
            if (parameterSymbol.Type.Name == "QuestPDF.Infrastructure.Color")
                return $"(QuestPDF.Infrastructure.Color){parameterSymbol.Name}";

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
                PythonMethodReturnType = "dupa"
                
                
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
                    result += $" = {parameterSymbol.ExplicitDefaultValue}";
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
    }
}