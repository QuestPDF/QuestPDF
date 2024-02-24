using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkFontManager
{
    public IntPtr Instance { get; }
    
    public static SkFontManager Empty { get; } = new(API.font_manager_get_empty());
    public static SkFontManager Global { get; } = new(API.font_manager_create_default());

    private SkFontManager(IntPtr instance)
    {
        Instance = instance;
    }
    
    public SkTypeface CreateTypeface(SkData data)
    {
        var instance = API.font_manager_create_typeface(Instance, data.Instance);
        return new SkTypeface(instance, disposeNativeObject: false);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr font_manager_get_empty();
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr font_manager_create_default();
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr font_manager_create_typeface(IntPtr fontManager, IntPtr fontData);
    }
}