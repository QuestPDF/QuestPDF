using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace QuestPDF.InteropGenerators;

/// <summary>
/// Generates native AOT/C ABI FFI code and Python bindings for QuestPDF IContainer extension methods.
/// This enables fluent API usage in Python through C# interop.
/// </summary>
public class ContainerSourceGenerator : ISourceGenerator
{
    /// <summary>
    /// Generates C# UnmanagedCallersOnly methods for native AOT compilation with C ABI compatibility.
    /// Each IContainer extension method gets a corresponding FFI wrapper.
    /// </summary>
    public string GenerateCSharpCode(INamespaceSymbol namespaceSymbol)
    {
        var containerMethods = FindContainerExtensionMethods(namespaceSymbol);

        if (!containerMethods.Any())
            return "// No IContainer extension methods found";

        var code = new StringBuilder();

        // Generate header
        code.AppendLine("using System;");
        code.AppendLine("using System.Runtime.InteropServices;");
        code.AppendLine("using System.Runtime.CompilerServices;");
        code.AppendLine("using QuestPDF.Infrastructure;");
        code.AppendLine("using QuestPDF.Fluent;");
        code.AppendLine();
        code.AppendLine("namespace QuestPDF.InteropBindings");
        code.AppendLine("{");
        code.AppendLine("    /// <summary>");
        code.AppendLine("    /// Native AOT FFI bindings for IContainer extension methods");
        code.AppendLine("    /// </summary>");
        code.AppendLine("    public static class ContainerInterop");
        code.AppendLine("    {");

        // Generate handle management
        code.AppendLine("        private static readonly Dictionary<IntPtr, IContainer> ContainerHandles = new();");
        code.AppendLine("        private static IntPtr _nextHandle = (IntPtr)1;");
        code.AppendLine();
        code.AppendLine("        private static IntPtr AllocateHandle(IContainer container)");
        code.AppendLine("        {");
        code.AppendLine("            var handle = _nextHandle;");
        code.AppendLine("            _nextHandle = (IntPtr)((long)_nextHandle + 1);");
        code.AppendLine("            ContainerHandles[handle] = container;");
        code.AppendLine("            return handle;");
        code.AppendLine("        }");
        code.AppendLine();
        code.AppendLine("        private static IContainer GetContainer(IntPtr handle)");
        code.AppendLine("        {");
        code.AppendLine("            if (!ContainerHandles.TryGetValue(handle, out var container))");
        code.AppendLine("                throw new InvalidOperationException($\"Invalid container handle: {handle}\");");
        code.AppendLine("            return container;");
        code.AppendLine("        }");
        code.AppendLine();
        code.AppendLine("        [UnmanagedCallersOnly(EntryPoint = \"container_release\")]");
        code.AppendLine("        public static void ReleaseContainer(IntPtr handle)");
        code.AppendLine("        {");
        code.AppendLine("            ContainerHandles.Remove(handle);");
        code.AppendLine("        }");
        code.AppendLine();

        // Generate FFI methods for each extension method
        foreach (var method in containerMethods)
        {
            GenerateCSharpMethod(code, method);
        }

        code.AppendLine("    }");
        code.AppendLine("}");

        return code.ToString();
    }

    /// <summary>
    /// Generates Python bindings using CFFI for FFI calls to the C# native AOT library.
    /// Creates a Python Container class with fluent API methods.
    /// </summary>
    public string GeneratePythonCode(INamespaceSymbol namespaceSymbol)
    {
        var containerMethods = FindContainerExtensionMethods(namespaceSymbol);

        if (!containerMethods.Any())
            return "# No IContainer extension methods found";

        var code = new StringBuilder();

        // Generate imports and setup
        code.AppendLine("from cffi import FFI");
        code.AppendLine("from typing import Optional, Callable, Any");
        code.AppendLine("from enum import IntEnum");
        code.AppendLine();
        code.AppendLine("# Initialize CFFI");
        code.AppendLine("ffi = FFI()");
        code.AppendLine();
        code.AppendLine("# Define C function signatures");
        code.AppendLine("ffi.cdef(\"\"\"");

        // Generate C function declarations for CFFI
        code.AppendLine("    void container_release(void* handle);");
        foreach (var method in containerMethods)
        {
            GenerateCFFISignature(code, method);
        }

        code.AppendLine("\"\"\")");
        code.AppendLine();
        code.AppendLine("# Load the native library");
        code.AppendLine("_lib = ffi.dlopen('./QuestPDF.Native.dll')  # Adjust path as needed");
        code.AppendLine();
        code.AppendLine("class Container:");
        code.AppendLine("    \"\"\"");
        code.AppendLine("    Represents a layout structure with exactly one child element.");
        code.AppendLine("    Provides fluent API for building QuestPDF documents.");
        code.AppendLine("    \"\"\"");
        code.AppendLine();
        code.AppendLine("    def __init__(self, handle):");
        code.AppendLine("        \"\"\"Initialize container with native handle\"\"\"");
        code.AppendLine("        self._handle = handle");
        code.AppendLine();
        code.AppendLine("    def __del__(self):");
        code.AppendLine("        \"\"\"Release native resources\"\"\"");
        code.AppendLine("        if hasattr(self, '_handle') and self._handle:");
        code.AppendLine("            _lib.container_release(self._handle)");
        code.AppendLine();
        code.AppendLine("    @property");
        code.AppendLine("    def handle(self):");
        code.AppendLine("        \"\"\"Get the native handle\"\"\"");
        code.AppendLine("        return self._handle");
        code.AppendLine();

        // Generate Python methods for each extension method
        foreach (var method in containerMethods)
        {
            GeneratePythonMethod(code, method);
        }

        return code.ToString();
    }

    private List<IMethodSymbol> FindContainerExtensionMethods(INamespaceSymbol namespaceSymbol)
    {
        var methods = new List<IMethodSymbol>();
        FindExtensionMethodsRecursive(namespaceSymbol, methods);
        return methods.Where(m => IsContainerExtensionMethod(m)).ToList();
    }

    private void FindExtensionMethodsRecursive(INamespaceSymbol namespaceSymbol, List<IMethodSymbol> methods)
    {
        // Search in current namespace types
        foreach (var type in namespaceSymbol.GetTypeMembers())
        {
            if (type.IsStatic)
            {
                foreach (var member in type.GetMembers().OfType<IMethodSymbol>())
                {
                    if (member.IsExtensionMethod)
                    {
                        methods.Add(member);
                    }
                }
            }
        }

        // Recursively search child namespaces
        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            FindExtensionMethodsRecursive(childNamespace, methods);
        }
    }

    private bool IsContainerExtensionMethod(IMethodSymbol method)
    {
        if (!method.IsExtensionMethod)
            return false;

        var firstParam = method.Parameters.FirstOrDefault();
        if (firstParam == null)
            return false;

        // Check if the first parameter is IContainer
        var paramType = firstParam.Type;
        return paramType.Name == "IContainer" &&
               paramType.ContainingNamespace?.ToDisplayString() == "QuestPDF.Infrastructure";
    }

    private void GenerateCSharpMethod(StringBuilder code, IMethodSymbol method)
    {
        var methodName = ToSnakeCaseLower(method.Name);
        var entryPoint = $"container_{methodName}";

        code.AppendLine($"        [UnmanagedCallersOnly(EntryPoint = \"{entryPoint}\")]");

        // Generate method signature
        var returnType = method.ReturnsVoid ? "void" : "IntPtr";
        code.Append($"        public static {returnType} {ToPascalCase(method.Name)}(IntPtr containerHandle");

        // Add parameters (skip the first one as it's the extension method's 'this' parameter)
        foreach (var param in method.Parameters.Skip(1))
        {
            code.Append($", {GetCSharpFFIType(param.Type)} {param.Name}");
        }
        code.AppendLine(")");
        code.AppendLine("        {");

        // Generate method body
        code.AppendLine("            try");
        code.AppendLine("            {");
        code.AppendLine("                var container = GetContainer(containerHandle);");

        // Generate the actual method call
        var callParams = string.Join(", ", method.Parameters.Skip(1).Select(p => ConvertFromFFI(p)));

        if (method.ReturnsVoid)
        {
            code.AppendLine($"                container.{method.Name}({callParams});");
        }
        else if (IsContainerReturnType(method.ReturnType))
        {
            code.AppendLine($"                var result = container.{method.Name}({callParams});");
            code.AppendLine("                return AllocateHandle(result);");
        }
        else
        {
            code.AppendLine($"                return container.{method.Name}({callParams});");
        }

        code.AppendLine("            }");
        code.AppendLine("            catch");
        code.AppendLine("            {");
        code.AppendLine(method.ReturnsVoid ? "                return;" : "                return IntPtr.Zero;");
        code.AppendLine("            }");
        code.AppendLine("        }");
        code.AppendLine();
    }

    private void GenerateCFFISignature(StringBuilder code, IMethodSymbol method)
    {
        var cFunctionName = $"container_{ToSnakeCaseLower(method.Name)}";

        // Generate return type
        var returnType = method.ReturnsVoid ? "void" : GetCFFIType(method.ReturnType);
        code.Append($"    {returnType} {cFunctionName}(void* handle");

        // Add parameters (skip the first one as it's the extension method's 'this' parameter)
        foreach (var param in method.Parameters.Skip(1))
        {
            code.Append($", {GetCFFIType(param.Type)} {ToSnakeCaseLower(param.Name)}");
        }

        code.AppendLine(");");
    }

    private void GeneratePythonMethod(StringBuilder code, IMethodSymbol method)
    {
        var pythonMethodName = ToSnakeCaseLower(method.Name);
        var doc = DocumentationHelper.ExtractDocumentation(method.GetDocumentationCommentXml());

        // Generate method signature
        code.Append($"    def {pythonMethodName}(self");

        // Add parameters
        foreach (var param in method.Parameters.Skip(1))
        {
            var paramName = ToSnakeCaseLower(param.Name);
            var pythonType = GetPythonType(param.Type);
            var defaultValue = GetPythonDefaultValue(param);
            code.Append($", {paramName}: {pythonType}{defaultValue}");
        }

        code.AppendLine("):");

        // Add docstring
        if (!string.IsNullOrEmpty(doc))
        {
            code.AppendLine("        \"\"\"");
            code.AppendLine($"        {doc}");

            // Add parameter documentation
            if (method.Parameters.Length > 1)
            {
                code.AppendLine();
                code.AppendLine("        Args:");
                foreach (var param in method.Parameters.Skip(1))
                {
                    var paramName = ToSnakeCaseLower(param.Name);
                    code.AppendLine($"            {paramName}: {GetPythonType(param.Type)}");
                }
            }

            // Add return documentation
            if (!method.ReturnsVoid && IsContainerReturnType(method.ReturnType))
            {
                code.AppendLine();
                code.AppendLine("        Returns:");
                code.AppendLine("            Container: Self for method chaining");
            }

            code.AppendLine("        \"\"\"");
        }

        // Generate method body
        var cFunctionName = $"container_{ToSnakeCaseLower(method.Name)}";
        var callParams = "self._handle";

        foreach (var param in method.Parameters.Skip(1))
        {
            var paramName = ToSnakeCaseLower(param.Name);
            callParams += $", {ConvertToCFFI(param, paramName)}";
        }

        if (method.ReturnsVoid)
        {
            code.AppendLine($"        _lib.{cFunctionName}({callParams})");
            code.AppendLine("        return self");
        }
        else if (IsContainerReturnType(method.ReturnType))
        {
            code.AppendLine($"        new_handle = _lib.{cFunctionName}({callParams})");
            code.AppendLine("        if new_handle != ffi.NULL:");
            code.AppendLine("            return Container(new_handle)");
            code.AppendLine("        return self");
        }
        else
        {
            code.AppendLine($"        return _lib.{cFunctionName}({callParams})");
        }

        code.AppendLine();
    }

    private string GetCFFIType(ITypeSymbol type)
    {
        if (IsContainerReturnType(type))
            return "void*";

        return type.SpecialType switch
        {
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Int32 => "int",
            SpecialType.System_Single => "float",
            SpecialType.System_Double => "double",
            SpecialType.System_String => "char*",
            _ when type.TypeKind == TypeKind.Enum => "int",
            _ => "void*"
        };
    }

    private string GetCSharpFFIType(ITypeSymbol type)
    {
        return type.SpecialType switch
        {
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Int32 => "int",
            SpecialType.System_Single => "float",
            SpecialType.System_Double => "double",
            SpecialType.System_String => "IntPtr", // Marshalled as char*
            _ when type.TypeKind == TypeKind.Enum => "int",
            _ => "IntPtr"
        };
    }

    private string GetPythonType(ITypeSymbol type)
    {
        return type.SpecialType switch
        {
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Int32 => "int",
            SpecialType.System_Single => "float",
            SpecialType.System_Double => "float",
            SpecialType.System_String => "str",
            _ when type.TypeKind == TypeKind.Enum => "int",
            _ when type.Name == "Action" || type.Name == "Func" => "Callable",
            _ => "Any"
        };
    }

    private string ConvertToCFFI(IParameterSymbol param, string paramName)
    {
        if (param.Type.SpecialType == SpecialType.System_String)
        {
            return $"{paramName}.encode('utf-8') if isinstance({paramName}, str) else {paramName}";
        }

        return paramName;
    }

    private string GetPythonDefaultValue(IParameterSymbol param)
    {
        if (!param.HasExplicitDefaultValue)
            return "";

        if (param.ExplicitDefaultValue == null)
            return " = None";

        return param.Type.SpecialType switch
        {
            SpecialType.System_Boolean => $" = {param.ExplicitDefaultValue.ToString().ToLower()}",
            SpecialType.System_Int32 or SpecialType.System_Single or SpecialType.System_Double => $" = {param.ExplicitDefaultValue}",
            SpecialType.System_String => $" = \"{param.ExplicitDefaultValue}\"",
            _ => ""
        };
    }

    private string ConvertFromFFI(IParameterSymbol param)
    {
        if (param.Type.SpecialType == SpecialType.System_String)
        {
            return $"Marshal.PtrToStringUTF8({param.Name})";
        }

        if (param.Type.TypeKind == TypeKind.Enum)
        {
            return $"({param.Type.Name}){param.Name}";
        }

        return param.Name;
    }


    private bool IsContainerReturnType(ITypeSymbol type)
    {
        return type.Name == "IContainer" &&
               type.ContainingNamespace?.ToDisplayString() == "QuestPDF.Infrastructure";
    }

    private string ToSnakeCaseLower(string pascalCase)
    {
        if (string.IsNullOrEmpty(pascalCase))
            return pascalCase;

        var result = new StringBuilder();
        result.Append(char.ToLower(pascalCase[0]));

        for (int i = 1; i < pascalCase.Length; i++)
        {
            if (char.IsUpper(pascalCase[i]))
            {
                result.Append('_');
                result.Append(char.ToLower(pascalCase[i]));
            }
            else
            {
                result.Append(pascalCase[i]);
            }
        }

        return result.ToString();
    }

    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }
}