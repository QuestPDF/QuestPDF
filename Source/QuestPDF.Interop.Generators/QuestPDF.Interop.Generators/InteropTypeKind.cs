using Microsoft.CodeAnalysis;

namespace QuestPDF.Interop.Generators;

internal enum InteropTypeKind
{
    Void,
    Boolean,
    Int,
    Float,
    String,
    Enum,
    Class,
    Interface,
    Action,
    Func,
    TypeParameter,
    Color,
    ByteArray,
    Size,
    ImageSize,
    Unknown
}

internal static class InteropTypeClassification
{
    public static InteropTypeKind GetInteropTypeKind(this ITypeSymbol type)
    {
        // primitives (by SpecialType)
        if (type.SpecialType == SpecialType.System_Void)
            return InteropTypeKind.Void;

        if (type.SpecialType == SpecialType.System_Boolean)
            return InteropTypeKind.Boolean;

        if (type.SpecialType == SpecialType.System_String)
            return InteropTypeKind.String;

        if (type.SpecialType is SpecialType.System_Int32 or SpecialType.System_UInt32
            or SpecialType.System_Int64 or SpecialType.System_UInt64
            or SpecialType.System_Int16 or SpecialType.System_UInt16
            or SpecialType.System_Byte or SpecialType.System_SByte)
            return InteropTypeKind.Int;

        if (type.SpecialType is SpecialType.System_Single or SpecialType.System_Double)
            return InteropTypeKind.Float;

        // simple type categories
        if (type.TypeKind == TypeKind.Enum)
            return InteropTypeKind.Enum;

        if (type.TypeKind == TypeKind.TypeParameter)
            return InteropTypeKind.TypeParameter;

        // QuestPDF domain types (by qualified name; checked before Class/Interface to take priority)
        var qualifiedName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        if (qualifiedName.Contains("QuestPDF.Infrastructure.Color"))
            return InteropTypeKind.Color;

        if (qualifiedName.Contains("QuestPDF.Infrastructure.ImageSize"))
            return InteropTypeKind.ImageSize;

        if (qualifiedName.Contains("QuestPDF.Infrastructure.Size"))
            return InteropTypeKind.Size;

        if (type.ToDisplayString() == "byte[]")
            return InteropTypeKind.ByteArray;

        // delegates
        if (type.IsAction())
            return InteropTypeKind.Action;

        if (type.IsFunc())
            return InteropTypeKind.Func;

        // Reference type categories
        if (type.TypeKind == TypeKind.Interface)
            return InteropTypeKind.Interface;

        if (type.TypeKind == TypeKind.Class)
            return InteropTypeKind.Class;

        return InteropTypeKind.Unknown;
    }

    public static bool IsAction(this ITypeSymbol typeSymbol)
    {
        return typeSymbol
            .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .StartsWith("global::System.Action");
    }

    public static bool IsFunc(this ITypeSymbol typeSymbol)
    {
        return typeSymbol
            .ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .StartsWith("global::System.Func");
    }
}
