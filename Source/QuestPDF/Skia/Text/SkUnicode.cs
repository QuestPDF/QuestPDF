using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkUnicode
{
    public IntPtr Instance { get; private set; }
    public static SkUnicode Global { get; } = new();

    private SkUnicode()
    {
        Instance = API.unicode_create();
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr unicode_create();
    }
}