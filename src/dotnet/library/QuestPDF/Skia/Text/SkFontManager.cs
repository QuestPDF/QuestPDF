using System;
using System.Linq;
using System.Runtime.InteropServices;
using QuestPDF.Helpers;

namespace QuestPDF.Skia.Text;

internal sealed class SkFontManager
{
    public IntPtr Instance { get; }
    
    public static SkFontManager Local { get; } = new(API.questpdf_skia_font_manager_create_local(Settings.FontDiscoveryPaths.FirstOrDefault() ?? PathHelpers.ApplicationFilesPath));
    public static SkFontManager Global { get; } = new(API.questpdf_skia_font_manager_create_global());

    private SkFontManager(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public SkTypeface CreateTypeface(SkData data)
    {
        var instance = API.questpdf_skia_font_manager_create_typeface(Instance, data.Instance);
        
        if (instance == IntPtr.Zero)
            throw new Exception("Cannot decode the provided font file.");
        
        return new SkTypeface(instance);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_font_manager_create_local(string path);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_font_manager_create_global();
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_font_manager_create_typeface(IntPtr fontManager, IntPtr fontData);
    }
}