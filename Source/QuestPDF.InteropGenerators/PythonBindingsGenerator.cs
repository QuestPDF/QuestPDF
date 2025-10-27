using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace QuestPDF.InteropGenerators;

/// <summary>
/// Generates Python ctypes bindings for interop
/// </summary>
public static class PythonBindingsGenerator
{
    /// <summary>
    /// Generates the complete Python bindings code
    /// </summary>
    public static string GeneratePythonBindings(List<IMethodSymbol> extensionMethods)
    {
        var sb = new StringBuilder();
        
        // Generate header and base classes
        GeneratePythonHeader(sb);
        
        // Group methods by their containing type
        var methodsByType = GroupMethodsByType(extensionMethods);
        
        // Generate the main library class
        GenerateLibraryClass(sb, extensionMethods);
        
        // Generate Python wrapper classes for each C# type
        GeneratePythonWrapperClasses(sb, methodsByType);
        
        // Comment out all lines with "//" to avoid C# compilation issues
        return CommentOutPythonCode(sb.ToString());
    }
    
    /// <summary>
    /// Comments out all Python code lines with "//" prefix to avoid C# compilation issues
    /// </summary>
    private static string CommentOutPythonCode(string pythonCode)
    {
        var lines = pythonCode.Split(new[] { "\r\n", "\r", "\n" }, System.StringSplitOptions.None);
        var commentedLines = lines.Select(line => "// " + line);
        return string.Join("\n", commentedLines);
    }
    
    private static void GeneratePythonHeader(StringBuilder sb)
    {
        sb.AppendLine("# Auto-generated Python bindings for QuestPDF");
        sb.AppendLine("# This file provides ctypes-based wrapper for the QuestPDF interop layer");
        sb.AppendLine();
        sb.AppendLine("import ctypes");
        sb.AppendLine("import platform");
        sb.AppendLine("from typing import Optional, TYPE_CHECKING");
        sb.AppendLine("from pathlib import Path");
        sb.AppendLine();
        sb.AppendLine("if TYPE_CHECKING:");
        sb.AppendLine("    from typing import Any");
        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine("class QuestPDFException(Exception):");
        sb.AppendLine("    \"\"\"Base exception for QuestPDF errors\"\"\"");
        sb.AppendLine("    pass");
        sb.AppendLine();
        sb.AppendLine();
    }
    
    private static Dictionary<string, List<IMethodSymbol>> GroupMethodsByType(List<IMethodSymbol> methods)
    {
        var result = new Dictionary<string, List<IMethodSymbol>>();
        
        foreach (var method in methods)
        {
            if (!PublicApiAnalyzer.IsSupported(method))
                continue;
            
            // For extension methods, use the first parameter type (the extended type)
            // For instance methods, use the containing type
            string typeName;
            if (method.IsExtensionMethod)
            {
                typeName = method.Parameters[0].Type.Name;
            }
            else
            {
                typeName = method.ContainingType.Name;
            }
            
            if (!result.ContainsKey(typeName))
                result[typeName] = new List<IMethodSymbol>();
            
            result[typeName].Add(method);
        }
        
        return result;
    }
    
    private static void GenerateLibraryClass(StringBuilder sb, List<IMethodSymbol> extensionMethods)
    {
        sb.AppendLine("class QuestPDFLibrary:");
        sb.AppendLine("    \"\"\"Wrapper for QuestPDF native library\"\"\"");
        sb.AppendLine("    ");
        sb.AppendLine("    def __init__(self, library_path: Optional[str] = None):");
        sb.AppendLine("        \"\"\"");
        sb.AppendLine("        Initialize QuestPDF library.");
        sb.AppendLine("        ");
        sb.AppendLine("        Args:");
        sb.AppendLine("            library_path: Path to the native library. If None, will search in standard locations.");
        sb.AppendLine("        \"\"\"");
        sb.AppendLine("        if library_path is None:");
        sb.AppendLine("            library_path = self._find_library()");
        sb.AppendLine("        ");
        sb.AppendLine("        self._lib = ctypes.CDLL(library_path)");
        sb.AppendLine("        self._setup_functions()");
        sb.AppendLine("    ");
        sb.AppendLine("    @staticmethod");
        sb.AppendLine("    def _find_library() -> str:");
        sb.AppendLine("        \"\"\"Find the QuestPDF library in standard locations\"\"\"");
        sb.AppendLine("        system = platform.system()");
        sb.AppendLine("        ");
        sb.AppendLine("        if system == 'Windows':");
        sb.AppendLine("            lib_name = 'QuestPDF.dll'");
        sb.AppendLine("        elif system == 'Darwin':");
        sb.AppendLine("            lib_name = 'QuestPDF.dylib'");
        sb.AppendLine("        else:");
        sb.AppendLine("            lib_name = 'QuestPDF.so'");
        sb.AppendLine("        ");
        sb.AppendLine("        # Search in common locations");
        sb.AppendLine("        search_paths = [");
        sb.AppendLine("            Path.cwd() / lib_name,");
        sb.AppendLine("            Path(__file__).parent / lib_name,");
        sb.AppendLine("            Path(__file__).parent / 'bin' / lib_name,");
        sb.AppendLine("        ]");
        sb.AppendLine("        ");
        sb.AppendLine("        for path in search_paths:");
        sb.AppendLine("            if path.exists():");
        sb.AppendLine("                return str(path)");
        sb.AppendLine("        ");
        sb.AppendLine("        raise QuestPDFException(f'Could not find {lib_name} in any standard location')");
        sb.AppendLine("    ");
        sb.AppendLine("    def _setup_functions(self):");
        sb.AppendLine("        \"\"\"Setup function signatures for all exported functions\"\"\"");
        sb.AppendLine("        ");
        sb.AppendLine("        # Setup free_handle");
        sb.AppendLine("        self._lib.questpdf_free_handle.argtypes = [ctypes.c_void_p]");
        sb.AppendLine("        self._lib.questpdf_free_handle.restype = None");
        sb.AppendLine();
        
        // Generate function setup for each method
        foreach (var method in extensionMethods)
        {
            if (!PublicApiAnalyzer.IsSupported(method))
                continue;
                
            GeneratePythonFunctionSetup(sb, method);
        }
        
        sb.AppendLine();
        sb.AppendLine("    def free_handle(self, handle: int):");
        sb.AppendLine("        \"\"\"Free a handle to a managed object\"\"\"");
        sb.AppendLine("        if handle != 0:");
        sb.AppendLine("            self._lib.questpdf_free_handle(handle)");
        sb.AppendLine();
        sb.AppendLine();
    }
    
    private static void GeneratePythonWrapperClasses(StringBuilder sb, Dictionary<string, List<IMethodSymbol>> methodsByType)
    {
        // Sort types alphabetically for consistent output
        var sortedTypes = methodsByType.Keys.OrderBy(k => k).ToList();
        
        foreach (var typeName in sortedTypes)
        {
            var methods = methodsByType[typeName];
            var pythonClassName = ToPythonClassName(typeName);
            
            sb.AppendLine($"class {pythonClassName}:");
            sb.AppendLine($"    \"\"\"Python wrapper for {typeName}\"\"\"");
            sb.AppendLine("    ");
            sb.AppendLine("    def __init__(self, lib: QuestPDFLibrary, handle: int):");
            sb.AppendLine("        self._lib = lib");
            sb.AppendLine("        self._handle = handle");
            sb.AppendLine("    ");
            sb.AppendLine("    @property");
            sb.AppendLine("    def handle(self) -> int:");
            sb.AppendLine("        \"\"\"Get the underlying native handle\"\"\"");
            sb.AppendLine("        return self._handle");
            sb.AppendLine("    ");
            sb.AppendLine("    def __del__(self):");
            sb.AppendLine("        if hasattr(self, '_handle') and self._handle != 0:");
            sb.AppendLine("            try:");
            sb.AppendLine("                self._lib.free_handle(self._handle)");
            sb.AppendLine("            except:");
            sb.AppendLine("                pass  # Ignore errors during cleanup");
            sb.AppendLine("    ");
            sb.AppendLine("    def __enter__(self):");
            sb.AppendLine("        return self");
            sb.AppendLine("    ");
            sb.AppendLine("    def __exit__(self, exc_type, exc_val, exc_tb):");
            sb.AppendLine("        self._lib.free_handle(self._handle)");
            sb.AppendLine("        self._handle = 0");
            sb.AppendLine();
            
            // Generate methods for this class
            foreach (var method in methods.OrderBy(m => m.Name))
            {
                GeneratePythonClassMethod(sb, method, pythonClassName);
            }
            
            sb.AppendLine();
        }
    }
    
    private static void GeneratePythonFunctionSetup(StringBuilder sb, IMethodSymbol method)
    {
        var entryPoint = CSharpInteropGenerator.GenerateEntryPointName(method);
        var isInstanceMethod = !method.IsStatic && !method.IsExtensionMethod;
        
        sb.AppendLine($"        # {method.ContainingType.Name}.{method.Name}");
        sb.Append($"        self._lib.{entryPoint}.argtypes = [");
        
        var argTypes = new List<string>();
        
        // For instance methods, add 'this' parameter
        if (isInstanceMethod)
        {
            argTypes.Add("ctypes.c_void_p");
        }
        
        foreach (var param in method.Parameters)
        {
            argTypes.Add(GetPythonCType(param.Type));
        }
        
        sb.Append(string.Join(", ", argTypes));
        sb.AppendLine("]");
        sb.AppendLine($"        self._lib.{entryPoint}.restype = {GetPythonCType(method.ReturnType)}");
        sb.AppendLine();
    }
    
    private static void GeneratePythonClassMethod(StringBuilder sb, IMethodSymbol method, string pythonClassName)
    {
        var entryPoint = CSharpInteropGenerator.GenerateEntryPointName(method);
        var pythonName = ToPythonMethodName(method.Name);
        var isExtensionMethod = method.IsExtensionMethod;
        
        // Build parameter list
        var parameters = new List<string> { "self" };
        
        // For extension methods, skip the first parameter (the extended type - that's 'self')
        var paramsToProcess = isExtensionMethod ? method.Parameters.Skip(1) : method.Parameters;
        
        foreach (var param in paramsToProcess)
        {
            var paramName = ToPythonParamName(param.Name);
            var pythonType = GetPythonTypeHint(param.Type, isParameter: true);
            parameters.Add($"{paramName}: {pythonType}");
        }
        
        var returnTypeHint = GetPythonTypeHint(method.ReturnType, isParameter: false);
        
        sb.AppendLine($"    def {pythonName}({string.Join(", ", parameters)}) -> '{returnTypeHint}':");
        sb.AppendLine($"        \"\"\"");
        sb.AppendLine($"        {method.Name}");
        if (!string.IsNullOrEmpty(method.GetDocumentationCommentXml()))
        {
            // Could extract summary from XML here if needed
        }
        sb.AppendLine($"        \"\"\"");
        
        // Build argument list for the call
        var callArgs = new List<string>();
        
        // Always pass 'self._handle' as the first argument (either for extension method or instance method)
        callArgs.Add("self._handle");
        
        foreach (var param in paramsToProcess)
        {
            var paramName = ToPythonParamName(param.Name);
            if (PublicApiAnalyzer.IsReferenceType(param.Type))
            {
                callArgs.Add($"{paramName}.handle if hasattr({paramName}, 'handle') else {paramName}");
            }
            else
            {
                callArgs.Add(paramName);
            }
        }
        
        if (method.ReturnsVoid)
        {
            sb.AppendLine($"        self._lib._lib.{entryPoint}({string.Join(", ", callArgs)})");
        }
        else if (PublicApiAnalyzer.IsReferenceType(method.ReturnType))
        {
            sb.AppendLine($"        result = self._lib._lib.{entryPoint}({string.Join(", ", callArgs)})");
            var returnPythonClass = ToPythonClassName(method.ReturnType.Name);
            sb.AppendLine($"        return {returnPythonClass}(self._lib, result)");
        }
        else
        {
            sb.AppendLine($"        return self._lib._lib.{entryPoint}({string.Join(", ", callArgs)})");
        }
        
        sb.AppendLine();
    }
    
    private static string GetPythonCType(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_Void)
            return "None";
        
        if (PublicApiAnalyzer.IsReferenceType(type))
            return "ctypes.c_void_p";
        
        return type.SpecialType switch
        {
            SpecialType.System_Boolean => "ctypes.c_bool",
            SpecialType.System_Byte => "ctypes.c_uint8",
            SpecialType.System_SByte => "ctypes.c_int8",
            SpecialType.System_Int16 => "ctypes.c_int16",
            SpecialType.System_UInt16 => "ctypes.c_uint16",
            SpecialType.System_Int32 => "ctypes.c_int32",
            SpecialType.System_UInt32 => "ctypes.c_uint32",
            SpecialType.System_Int64 => "ctypes.c_int64",
            SpecialType.System_UInt64 => "ctypes.c_uint64",
            SpecialType.System_Single => "ctypes.c_float",
            SpecialType.System_Double => "ctypes.c_double",
            SpecialType.System_IntPtr => "ctypes.c_void_p",
            SpecialType.System_UIntPtr => "ctypes.c_void_p",
            _ => "ctypes.c_void_p"
        };
    }
    
    private static string GetPythonTypeHint(ITypeSymbol type, bool isParameter)
    {
        if (type.SpecialType == SpecialType.System_Void)
            return "None";
        
        if (PublicApiAnalyzer.IsReferenceType(type))
        {
            // Return the appropriate Python class name for reference types
            return ToPythonClassName(type.Name);
        }
        
        return type.SpecialType switch
        {
            SpecialType.System_Boolean => "bool",
            SpecialType.System_Byte => "int",
            SpecialType.System_SByte => "int",
            SpecialType.System_Int16 => "int",
            SpecialType.System_UInt16 => "int",
            SpecialType.System_Int32 => "int",
            SpecialType.System_UInt32 => "int",
            SpecialType.System_Int64 => "int",
            SpecialType.System_UInt64 => "int",
            SpecialType.System_Single => "float",
            SpecialType.System_Double => "float",
            SpecialType.System_IntPtr => "int",
            SpecialType.System_UIntPtr => "int",
            _ => "int"
        };
    }
    
    private static string ToPythonClassName(string csharpTypeName)
    {
        // Convert C# type name to Python class name
        // Remove generic markers and sanitize
        var cleanName = csharpTypeName
            .Replace("`", "")
            .Replace("<", "_")
            .Replace(">", "_")
            .Replace(",", "_")
            .Trim('_');
        
        // Return as-is (PascalCase is acceptable in Python for class names)
        return cleanName;
    }
    
    private static string ToPythonMethodName(string name)
    {
        // Convert PascalCase to snake_case
        var result = new StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            if (i > 0 && char.IsUpper(name[i]) && !char.IsUpper(name[i - 1]))
                result.Append('_');
            result.Append(char.ToLowerInvariant(name[i]));
        }
        return result.ToString();
    }
    
    private static string ToPythonParamName(string name)
    {
        // Convert to snake_case and avoid Python keywords
        var result = ToPythonMethodName(name);
        
        // Avoid Python keywords
        if (result == "class" || result == "from" || result == "import" || result == "def" || 
            result == "return" || result == "if" || result == "else" || result == "for" ||
            result == "while" || result == "try" || result == "except" || result == "with")
        {
            result += "_";
        }
        
        return result;
    }
}

