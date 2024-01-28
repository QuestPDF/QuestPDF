using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkFontCollection : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkFontCollection(IntPtr instance)
    {
        Instance = instance;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CreateCommand
    {
        public IntPtr TypefaceProvider;
        [MarshalAs(UnmanagedType.I1)] public bool UseGlobalFonts;
        [MarshalAs(UnmanagedType.I1)] public bool EnableFontFallback;
    }

    public static SkFontCollection Create(SkTypefaceProvider? typefaceProvider = null, bool useGlobalFonts = false, bool enableFontFallback = false)
    {
        typefaceProvider ??= new SkTypefaceProvider();
        
        var command = new CreateCommand
        {
            TypefaceProvider = typefaceProvider.Instance,
            UseGlobalFonts = useGlobalFonts,
            EnableFontFallback = enableFontFallback
        };
        
        var instance = API.font_collection_create(command);
        return new SkFontCollection(instance);
    }
    
    ~SkFontCollection()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.font_collection_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr font_collection_create(CreateCommand command);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void font_collection_delete(IntPtr fontCollection);
    }
}