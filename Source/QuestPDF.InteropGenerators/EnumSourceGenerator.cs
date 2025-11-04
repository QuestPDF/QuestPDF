using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.InteropGenerators;

internal class EnumSourceGenerator(string targetNamespace) : ISourceGenerator
{
    private string TargetNamespace { get; } = targetNamespace;
    
    public string GenerateCSharpCode(INamespaceSymbol namespaceSymbol)
    {
        // Enums don't require C ABI interop code generation
        // They are passed as integers across the FFI boundary
        return string.Empty;
    }
    
    public string GeneratePythonCode(INamespaceSymbol namespaceSymbol)
    {
        var enumSymbol = FindEnumSymbol(namespaceSymbol, TargetNamespace);

        if (enumSymbol == null)
            return $"# Enum not found: {TargetNamespace}";

        var code = new System.Text.StringBuilder();

        // Generate Python enum class
        code.AppendLine($"class {enumSymbol.Name}(IntEnum):");

        // Add class docstring from enum documentation
        var enumDocumentation = DocumentationHelper.ExtractDocumentation(enumSymbol.GetDocumentationCommentXml());
        if (!string.IsNullOrEmpty(enumDocumentation))
        {
            code.AppendLine("    \"\"\"");
            code.AppendLine($"    {enumDocumentation}");
            code.AppendLine("    \"\"\"");
            code.AppendLine();
        }

        var members = enumSymbol.GetMembers().OfType<IFieldSymbol>().ToList();

        if (members.Count == 0)
        {
            code.AppendLine("    pass");
        }
        else
        {
            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                var value = member.HasConstantValue ? member.ConstantValue : 0;

                // Add blank line between members for readability
                if (i > 0)
                    code.AppendLine();

                // Add member with value
                code.AppendLine($"    {NamingHelper.ToSnakeCase(member.Name)} = {value}");

                // Add member documentation as docstring (visible in IDE IntelliSense)
                var memberDoc = DocumentationHelper.ExtractDocumentation(member.GetDocumentationCommentXml());
                if (!string.IsNullOrEmpty(memberDoc))
                {
                    // Use triple-quoted docstring with consistent multi-line format
                    // This makes it visible in PyCharm and other IDE tooltips
                    code.AppendLine("    \"\"\"");

                    // Handle multi-line documentation
                    var docLines = memberDoc.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in docLines)
                    {
                        code.AppendLine($"    {line.Trim()}");
                    }

                    code.AppendLine("    \"\"\"");
                }
            }
        }

        code.AppendLine();
        return code.ToString();
    }

    private static INamedTypeSymbol? FindEnumSymbol(INamespaceSymbol rootNamespace, string fullyQualifiedName)
    {
        // Split the fully qualified name into parts
        var parts = fullyQualifiedName.Split('.');

        // Navigate to the target namespace
        var currentNamespace = rootNamespace;
        for (int i = 0; i < parts.Length - 1; i++)
        {
            var nextNamespace = currentNamespace.GetNamespaceMembers()
                .FirstOrDefault(ns => ns.Name == parts[i]);

            if (nextNamespace == null)
                return null;

            currentNamespace = nextNamespace;
        }

        // Find the enum type
        var enumName = parts[parts.Length - 1];
        return currentNamespace.GetTypeMembers()
            .FirstOrDefault(t => t.Name == enumName && t.TypeKind == TypeKind.Enum);
    }
}