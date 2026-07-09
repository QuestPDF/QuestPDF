using System;
using System.Diagnostics;
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
    
    public static void WarnThatFinalizerIsReached<T>(this T disposableObject) where T : IDisposable
    {
        Debug.Fail($"An object of type '{typeof(T).Name}' was not disposed explicitly, and was finalized instead.");
    }
}