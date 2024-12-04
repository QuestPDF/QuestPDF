using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkFontManager
{
    public IntPtr Instance { get; }
    
    public static SkFontManager Local { get; } = new(API.font_manager_create_local(Settings.FontDiscoveryPaths.FirstOrDefault() ?? Helpers.Helpers.ApplicationFilesPath));
    public static SkFontManager Global { get; } = new(API.font_manager_create_global());

    private SkFontManager(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public SkTypeface CreateTypeface(SkData data)
    {
        var instance = API.font_manager_create_typeface(Instance, data.Instance);
        
        if (instance == IntPtr.Zero)
            throw new Exception("Cannot decode the provided font file.");
        
        return new SkTypeface(instance);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr font_manager_create_local(string path);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr font_manager_create_global();
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr font_manager_create_typeface(IntPtr fontManager, IntPtr fontData);
    }
}