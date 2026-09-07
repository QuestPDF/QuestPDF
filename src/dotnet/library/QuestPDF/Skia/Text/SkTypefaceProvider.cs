using System;
using System.Runtime.InteropServices;
using QuestPDF.Infrastructure;

namespace QuestPDF.Skia.Text;

internal sealed class SkTypefaceProvider : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkTypefaceProvider()
    {
        Instance = API.questpdf_skia_typeface_font_provider_create();
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public void AddTypefaceFromData(SkData data, string? alias = null)
    {
        var registeredTypefaces = API.questpdf_skia_typeface_font_provider_add_typefaces_from_data(Instance, data.Instance, alias);
        
        if (registeredTypefaces == 0)
            throw new InvalidOperationException("QuestPDF cannot load the provided font data. Please make sure that it is a valid TrueType, OpenType or font collection file.");
    }
    
    public FontInfo[] GetTypefaces()
    {
        var fontManager = new SkFontManager(API.questpdf_skia_typeface_font_provider_as_font_manager(Instance));
        return fontManager.GetTypefaces();
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
        
        API.questpdf_skia_typeface_font_provider_unref(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_typeface_font_provider_create();
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_typeface_font_provider_as_font_manager(IntPtr typefaceProvider);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int questpdf_skia_typeface_font_provider_add_typefaces_from_data(IntPtr typefaceProvider, IntPtr data, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string? alias);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_typeface_font_provider_unref(IntPtr typefaceProvider);
    }
}
