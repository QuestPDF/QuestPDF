using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkTypefaceProvider : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkTypefaceProvider()
    {
        Instance = API.typeface_font_provider_create();
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public void AddTypefaceFromData(SkData data, string? alias = null)
    {
        var typeface = SkFontManager.Global.CreateTypeface(data);
        
        if (alias == null)
            API.typeface_font_provider_add_typeface(Instance, typeface.Instance);
        else
            API.typeface_font_provider_add_typeface_with_custom_alias(Instance, typeface.Instance, alias);
    }
    
    ~SkTypefaceProvider()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.typeface_font_provider_unref(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr typeface_font_provider_create();
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void typeface_font_provider_add_typeface(IntPtr typefaceProvider, IntPtr typeface);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void typeface_font_provider_add_typeface_with_custom_alias(IntPtr typefaceProvider, IntPtr typeface, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string alias);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void typeface_font_provider_unref(IntPtr typefaceProvider);
    }
}