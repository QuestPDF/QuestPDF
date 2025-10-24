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
        
        sb.AppendLine("# Auto-generated Python bindings for QuestPDF");
        sb.AppendLine("# This file provides ctypes-based wrapper for the QuestPDF interop layer");
        sb.AppendLine();
        sb.AppendLine("import ctypes");
        sb.AppendLine("import platform");
        sb.AppendLine("from typing import Optional");
        sb.AppendLine("from pathlib import Path");
        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine("class QuestPDFException(Exception):");
        sb.AppendLine("    \"\"\"Base exception for QuestPDF errors\"\"\"");
        sb.AppendLine("    pass");
        sb.AppendLine();
        sb.AppendLine();
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
        
        // Generate Python wrapper methods
        foreach (var method in extensionMethods)
        {
            if (!PublicApiAnalyzer.IsSupported(method))
                continue;
                
            GeneratePythonWrapperMethod(sb, method);
        }
        
        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine("class Handle:");
        sb.AppendLine("    \"\"\"Wrapper for a managed object handle with automatic cleanup\"\"\"");
        sb.AppendLine("    ");
        sb.AppendLine("    def __init__(self, lib: QuestPDFLibrary, handle: int):");
        sb.AppendLine("        self._lib = lib");
        sb.AppendLine("        self._handle = handle");
        sb.AppendLine("    ");
        sb.AppendLine("    @property");
        sb.AppendLine("    def value(self) -> int:");
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
        
        return sb.ToString();
    }
    
    private static void GeneratePythonFunctionSetup(StringBuilder sb, IMethodSymbol method)
    {
        var entryPoint = CSharpInteropGenerator.GenerateEntryPointName(method);
        
        sb.AppendLine($"        # {method.ContainingType.Name}.{method.Name}");
        sb.Append($"        self._lib.{entryPoint}.argtypes = [");
        
        var argTypes = new List<string>();
        foreach (var param in method.Parameters)
        {
            argTypes.Add(GetPythonCType(param.Type));
        }
        
        sb.Append(string.Join(", ", argTypes));
        sb.AppendLine("]");
        sb.AppendLine($"        self._lib.{entryPoint}.restype = {GetPythonCType(method.ReturnType)}");
        sb.AppendLine();
    }
    
    private static void GeneratePythonWrapperMethod(StringBuilder sb, IMethodSymbol method)
    {
        var entryPoint = CSharpInteropGenerator.GenerateEntryPointName(method);
        var pythonName = ToPythonMethodName(method.Name);
        
        // Build parameter list
        var parameters = new List<string> { "self" };
        foreach (var param in method.Parameters)
        {
            var paramName = ToPythonParamName(param.Name);
            var pythonType = GetPythonTypeHint(param.Type);
            parameters.Add($"{paramName}: {pythonType}");
        }
        
        var returnTypeHint = GetPythonTypeHint(method.ReturnType);
        
        sb.AppendLine($"    def {pythonName}({string.Join(", ", parameters)}) -> {returnTypeHint}:");
        sb.AppendLine($"        \"\"\"");
        sb.AppendLine($"        {method.ContainingType.Name}.{method.Name}");
        sb.AppendLine($"        \"\"\"");
        
        // Build argument list for the call
        var callArgs = new List<string>();
        foreach (var param in method.Parameters)
        {
            var paramName = ToPythonParamName(param.Name);
            if (PublicApiAnalyzer.IsReferenceType(param.Type))
            {
                callArgs.Add($"{paramName}.value if isinstance({paramName}, Handle) else {paramName}");
            }
            else
            {
                callArgs.Add(paramName);
            }
        }
        
        if (method.ReturnsVoid)
        {
            sb.AppendLine($"        self._lib.{entryPoint}({string.Join(", ", callArgs)})");
        }
        else if (PublicApiAnalyzer.IsReferenceType(method.ReturnType))
        {
            sb.AppendLine($"        result = self._lib.{entryPoint}({string.Join(", ", callArgs)})");
            sb.AppendLine($"        return Handle(self, result)");
        }
        else
        {
            sb.AppendLine($"        return self._lib.{entryPoint}({string.Join(", ", callArgs)})");
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
    
    private static string GetPythonTypeHint(ITypeSymbol type)
    {
        if (type.SpecialType == SpecialType.System_Void)
            return "None";
        
        if (PublicApiAnalyzer.IsReferenceType(type))
            return "Handle";
        
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

