using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace QuestPDF.InteropGenerators;

/// <summary>
/// Analyzes the QuestPDF public API and extracts methods suitable for interop
/// </summary>
public static class PublicApiAnalyzer
{
    /// <summary>
    /// Collects all public extension methods from the assembly
    /// </summary>
    public static List<IMethodSymbol> CollectExtensionMethods(INamespaceSymbol rootNamespace)
    {
        var methods = new List<IMethodSymbol>();
        CollectExtensionMethodsRecursive(rootNamespace, methods);
        return methods;
    }

    private static void CollectExtensionMethodsRecursive(INamespaceSymbol ns, List<IMethodSymbol> methods)
    {
        foreach (var member in ns.GetMembers())
        {
            if (member is INamespaceSymbol childNs)
            {
                CollectExtensionMethodsRecursive(childNs, methods);
            }
            else if (member is INamedTypeSymbol type)
            {
                CollectFromType(type, methods);
            }
        }
    }

    private static void CollectFromType(INamedTypeSymbol type, List<IMethodSymbol> methods)
    {
        if (type.DeclaredAccessibility != Accessibility.Public || type.IsImplicitlyDeclared)
            return;

        foreach (var member in type.GetMembers())
        {
            if (member is IMethodSymbol { MethodKind: MethodKind.Ordinary, IsExtensionMethod: true, DeclaredAccessibility: Accessibility.Public } method 
                && !method.IsImplicitlyDeclared)
            {
                methods.Add(method);
            }
        }

        foreach (var nestedType in type.GetTypeMembers())
        {
            CollectFromType(nestedType, methods);
        }
    }

    /// <summary>
    /// Checks if a method is supported for interop generation
    /// </summary>
    public static bool IsSupported(IMethodSymbol method)
    {
        // Async methods are not supported
        if (method.IsAsync)
            return false;
        
        // Check return type
        if (!IsSupportedType(method.ReturnType))
            return false;
        
        // Exclude methods that return Task or Task<T>
        if (IsTaskRelatedType(method.ReturnType))
            return false;
        
        // Check all parameter types
        foreach (var parameter in method.Parameters)
        {
            if (!IsSupportedType(parameter.Type))
                return false;
            
            // Exclude methods with Task-related parameters
            if (IsTaskRelatedType(parameter.Type))
                return false;
            
            // Ref/out parameters are not supported in UnmanagedCallersOnly
            if (parameter.RefKind != RefKind.None)
                return false;
        }
        
        // Generic methods are not supported
        if (method.IsGenericMethod)
            return false;
        
        return true;
    }

    /// <summary>
    /// Checks if a type is supported for interop
    /// </summary>
    public static bool IsSupportedType(ITypeSymbol type)
    {
        // Void is supported
        if (type.SpecialType == SpecialType.System_Void)
            return true;
        
        // Blittable primitive types
        if (type.SpecialType is 
            SpecialType.System_Boolean or
            SpecialType.System_Byte or
            SpecialType.System_SByte or
            SpecialType.System_Int16 or
            SpecialType.System_UInt16 or
            SpecialType.System_Int32 or
            SpecialType.System_UInt32 or
            SpecialType.System_Int64 or
            SpecialType.System_UInt64 or
            SpecialType.System_Single or
            SpecialType.System_Double or
            SpecialType.System_IntPtr or
            SpecialType.System_UIntPtr)
            return true;
        
        // Pointers are supported
        if (type.TypeKind == TypeKind.Pointer)
            return true;
        
        // Function pointers are supported
        if (type.TypeKind == TypeKind.FunctionPointer)
            return true;
        
        // Value types (structs) that are blittable might be supported
        // For simplicity, we'll allow struct types but developers should ensure they're blittable
        if (type.TypeKind == TypeKind.Struct && !type.IsRefLikeType)
            return true;
        
        // Enums are supported (backed by primitive types)
        if (type.TypeKind == TypeKind.Enum)
            return true;
        
        // Classes and interfaces are supported via handle boxing (passed as nint)
        if (type.TypeKind is TypeKind.Class or TypeKind.Interface)
            return true;
        
        // Generic types, arrays, delegates are not supported
        if (type is INamedTypeSymbol { IsGenericType: true })
            return false;
        
        if (type.TypeKind == TypeKind.Delegate)
            return false;
        
        if (type.TypeKind == TypeKind.Array)
            return false;
        
        return false;
    }

    /// <summary>
    /// Checks if a type is a reference type (class or interface)
    /// </summary>
    public static bool IsReferenceType(ITypeSymbol type)
    {
        return type.TypeKind is TypeKind.Class or TypeKind.Interface;
    }

    /// <summary>
    /// Checks if a type is Task-related (Task, CancellationToken, etc.)
    /// </summary>
    public static bool IsTaskRelatedType(ITypeSymbol type)
    {
        var fullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        
        // Check for Task, Task<T>, ValueTask, ValueTask<T>
        if (fullName.StartsWith("global::System.Threading.Tasks.Task") ||
            fullName.StartsWith("global::System.Threading.Tasks.ValueTask"))
            return true;
        
        // Check for CancellationToken
        if (fullName == "global::System.Threading.CancellationToken")
            return true;
        
        // Check for IAsyncEnumerable<T> and IAsyncEnumerator<T>
        if (fullName.StartsWith("global::System.Collections.Generic.IAsyncEnumerable") ||
            fullName.StartsWith("global::System.Collections.Generic.IAsyncEnumerator"))
            return true;
        
        return false;
    }
}

