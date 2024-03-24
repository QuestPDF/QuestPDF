using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkFontCollection : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkFontCollection(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct CreateCommand
    {
        public IntPtr FontManager;
        public IntPtr TypefaceProvider;
    }

    public static SkFontCollection Create(SkTypefaceProvider typefaceProvider, SkFontManager fontManager)
    {
        var command = new CreateCommand
        {
            FontManager = fontManager.Instance,
            TypefaceProvider = typefaceProvider.Instance
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
        
        API.font_collection_unref(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr font_collection_create(CreateCommand command);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void font_collection_unref(IntPtr fontCollection);
    }
}