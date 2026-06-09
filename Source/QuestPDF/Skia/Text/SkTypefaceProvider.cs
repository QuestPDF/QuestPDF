using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkTypefaceProvider : IDisposable
{
    public IntPtr Instance { get; private set; }
    private List<SkTypeface> Typefaces { get; } = new();
    
    public SkTypefaceProvider()
    {
        Instance = API.questpdf_skia_typeface_font_provider_create();
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public void AddTypefaceFromData(SkData data, string? alias = null)
    {
        var typeface = SkFontManager.Global.CreateTypeface(data);
        Typefaces.Add(typeface);
        
        if (alias == null)
            API.questpdf_skia_typeface_font_provider_add_typeface(Instance, typeface.Instance);
        else
            API.questpdf_skia_typeface_font_provider_add_typeface_with_custom_alias(Instance, typeface.Instance, alias);
    }
    
    ~SkTypefaceProvider()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        foreach (var typeface in Typefaces)
            typeface.Dispose();
        
        API.questpdf_skia_typeface_font_provider_unref(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_typeface_font_provider_create();
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_typeface_font_provider_add_typeface(IntPtr typefaceProvider, IntPtr typeface);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_typeface_font_provider_add_typeface_with_custom_alias(IntPtr typefaceProvider, IntPtr typeface, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string alias);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_typeface_font_provider_unref(IntPtr typefaceProvider);
    }
}