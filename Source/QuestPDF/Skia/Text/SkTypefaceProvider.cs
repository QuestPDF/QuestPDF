using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkTypefaceProvider : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkTypefaceProvider()
    {
        Instance = API.typeface_font_provider_create();
    }
    
    public void AddTypefaceFromData(SkData data, string? alias = null)
    {
        if (alias == null)
            API.typeface_font_provider_add_typeface(Instance, data.Instance);
        else
            API.typeface_font_provider_add_typeface_with_custom_alias(Instance, data.Instance, alias);
    }
    
    ~SkTypefaceProvider()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.typeface_font_provider_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr typeface_font_provider_create();
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void typeface_font_provider_add_typeface(IntPtr typefaceProvider, IntPtr data);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void typeface_font_provider_add_typeface_with_custom_alias(IntPtr typefaceProvider, IntPtr data, string alias);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void typeface_font_provider_delete(IntPtr typefaceProvider);
    }
}