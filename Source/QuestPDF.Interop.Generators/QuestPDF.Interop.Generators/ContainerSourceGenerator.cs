using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal class ContainerSourceGenerator(string targetNamespace) : IInteropSourceGenerator
{
    private string TargetNamespace { get; } = targetNamespace;

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
        return compilation
            .GetTypeByMetadataName(targetNamespace)
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(m => m.DeclaredAccessibility == Accessibility.Public && m.IsStatic && m.IsExtensionMethod);
    }
    
    public string GenerateCSharpCode(Compilation compilation)
    {
        var model = new
        {
            Methods = GetTargetMethods(compilation).Select(MapMethod)
        };

        return ScribanTemplateLoader.LoadTemplate("Container.cs").Render(model);
        
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
        
        return string.Join("\n", headers);
    }
}