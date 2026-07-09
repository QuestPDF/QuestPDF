using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkUnicode
{
    public IntPtr Instance { get; private set; }
    public static SkUnicode Global { get; } = new();

    private SkUnicode()
    {
        Instance = API.questpdf_skia_unicode_create();
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_unicode_create();
    }
}