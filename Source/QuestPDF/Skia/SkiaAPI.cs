using System;
using QuestPDF.Drawing.Exceptions;

namespace QuestPDF.Skia;

internal static class SkiaAPI
{
    public const string LibraryName = "QuestPdfSkia";
    
    public static void EnsureNotNull(IntPtr instance)
    {
        if (instance == IntPtr.Zero)
            throw new InitializationException($"QuestPDF cannot instantiate native object.");
    }
}