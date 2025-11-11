using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Scriban;

namespace QuestPDF.Interop.Generators;

internal class ContainerSourceGenerator(string targetNamespace) : IInteropSourceGenerator
{
    private string TargetNamespace { get; } = targetNamespace;

    class MethodTemplateModel
    {
        public string NativeName { get; set; }
        public string ManagedName { get; set; }
        public string ApiName { get; set; }
        public string Parameters { get; set; }
        public string ReturnType { get; set; }
    }
    
    public string GenerateCSharpCode(Compilation compilation)
    {
        var member = compilation.GetTypeByMetadataName(targetNamespace);
        
        var templateString = LoadTemplate();
        
        var model = new
        {
            ClassName = "PaddingInterop",
            Methods = member.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.DeclaredAccessibility == Accessibility.Public && m.IsStatic && m.IsExtensionMethod)
                .Select(MapMethod)
                .ToList()
        };

        var template = Template.Parse(templateString);
        return template.Render(model);
        
        static object MapMethod(IMethodSymbol method)
        {
            var parametersList = method
                .Parameters
                .Select(MapMethodParameter);
            
            return new MethodTemplateModel
            {
                NativeName = GetNativeMethodName(method),
                ManagedName = method.Name,
                ApiName = method.Name,
                Parameters = string.Join(", ", parametersList),
                ReturnType = method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            };
        }

        static string MapMethodParameter(IParameterSymbol parameterSymbol)
        {
            return $"{GetNativeParameterType(parameterSymbol.Type)} {parameterSymbol.Name}";
        }
    }

    private string LoadTemplate()
    {
        using var stream = this
            .GetType()
            .Assembly
            .GetManifestResourceStream("QuestPDF.Interop.Generators.Templates.simple-extension-method.cs.liquid");
 
        using var streamReader = new StreamReader(stream);
        return streamReader.ReadToEnd();
    }
    
    public string GeneratePythonCode(Compilation compilation)
    {
        return string.Empty;
    }
    

    
    

    private static string GetNativeMethodName(IMethodSymbol methodSymbol)
    {
        return $"questpdf_{ToSnakeCase(methodSymbol.Name)}";
    }
    
    
    
    
    
    private static string GetNativeParameterType(ITypeSymbol typeSymbol)
    {
        var typeName = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        
        // is enum
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
            "System.IntPtr" => "void*",
            "System.UIntPtr" => "void*",
            _ when typeSymbol.TypeKind == TypeKind.Pointer => "void*",
            _ when typeSymbol.TypeKind == TypeKind.Class || typeSymbol.TypeKind == TypeKind.Interface => "void*",
            _ => "void*" // Default for unknown types
        };
    }
    
    
    
    
    
    private static readonly Regex SnakeCaseRegex = new Regex("(?<!^)([A-Z])", RegexOptions.Compiled);
    
    public static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return SnakeCaseRegex.Replace(input, "_$1").ToLowerInvariant();
    }
}