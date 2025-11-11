using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Scriban;
using static QuestPDF.Interop.Generators.Helpers;

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
        public string TargetObjectParameterName { get; set; }
        public IEnumerable<string> TargetMethodParameters { get; set; }
        public string ReturnType { get; set; }
    }
    
    public string GenerateCSharpCode(Compilation compilation)
    {
        var member = compilation.GetTypeByMetadataName(targetNamespace);

        var model = new
        {
            ClassName = "PaddingInterop",
            Methods = member.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public && m.IsStatic && m.IsExtensionMethod)
                .Select(MapMethod)
                .ToList()
        };

        return ScribanTemplateLoader.LoadTemplate("container.cs").Render(model);
        
        static object MapMethod(IMethodSymbol method)
        {
            return new MethodTemplateModel
            {
                NativeName = GetNativeMethodName(method),
                ManagedName = method.Name,
                ApiName = method.Name,
                MethodParameters = method.Parameters.Select(GetMethodParameter),
                TargetObjectParameterName = method.Parameters.First().Name,
                TargetMethodParameters = method.Parameters.Skip(1).Select(GetTargetMethodParameter),
                ReturnType = method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            };
        }

        static string GetMethodParameter(IParameterSymbol parameterSymbol)
        {
            return $"{GetInteropMethodParameterType(parameterSymbol.Type)} {parameterSymbol.Name}";
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
        return string.Empty;
    }
    

    
    

    private static string GetNativeMethodName(IMethodSymbol methodSymbol)
    {
        var targetType = methodSymbol.IsExtensionMethod 
            ? methodSymbol.Parameters.First().Type 
            : methodSymbol.ContainingType;

        var isInterface = targetType.TypeKind == TypeKind.Interface;
        var targetTypeName = isInterface ? targetType.Name.TrimStart('I') : targetType.Name; 
        
        return $"questpdf_{ToSnakeCase(targetTypeName)}_{ToSnakeCase(methodSymbol.Name)}_{methodSymbol.GetHashCode()}";
    }
    
    
    
    private static string GetInteropMethodParameterType(ITypeSymbol typeSymbol)
    {
        var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (typeSymbol.TypeKind == TypeKind.Enum)
            return "int";

        if (typeSymbol.TypeKind == TypeKind.Class)
            return "nint";
        
        if (typeSymbol.TypeKind == TypeKind.Interface)
            return "nint";
        
        return typeName;
    }
    
    private static string GetNativeParameterType(ITypeSymbol typeSymbol)
    {
        var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (typeSymbol.TypeKind == TypeKind.Enum)
            return "int32_t";
        
        return typeName switch
        {
            "int" or "System.Int32" => "int32_t",
            "uint" or "System.UInt32" => "uint32_t",
            "long" or "System.Int64" => "int64_t",
            "ulong" or "System.UInt64" => "uint64_t",
            "short" or "System.Int16" => "int16_t",
            "ushort" or "System.UInt16" => "uint16_t",
            "byte" or "System.Byte" => "uint8_t",
            "sbyte" or "System.SByte" => "int8_t",
            "float" or "System.Single" => "float",
            "double" or "System.Double" => "double",
            "bool" or "System.Boolean" => "uint8_t",
            "char" or "System.Char" => "uint16_t",
            "string" or "System.String" => "const char*",
            "System.IntPtr" => "nint",
            "System.UIntPtr" => "nuint",
            _ when typeSymbol.TypeKind == TypeKind.Pointer => "nint",
            _ when typeSymbol.TypeKind == TypeKind.Class || typeSymbol.TypeKind == TypeKind.Interface => "nint",
            _ => "nint" // Default for unknown types
        };
    }
}