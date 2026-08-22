using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkFontCollection : IDisposable
{
    public IntPtr Instance { get; private set; }

    private bool IsPooled { get; set; }
    
    public SkFontCollection(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }

    public static SkFontCollection Create(SkTypefaceProvider typefaceProvider, SkFontManager fontManager)
    {
        var instance = API.questpdf_skia_font_collection_create(fontManager.Instance, typefaceProvider.Instance);
        return new SkFontCollection(instance);
    }
    
    /// <summary>
    /// Marks the font collection as owned by a pooled <see cref="SkParagraphBuilder"/>,
    /// which shares its lifetime and therefore also its lack of an explicit disposal point.
    /// </summary>
    public void MarkAsPooled()
    {
        IsPooled = true;
    }
    
    ~SkFontCollection()
    {
        if (!IsPooled)
            this.WarnThatFinalizerIsReached();

        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.questpdf_skia_font_collection_unref(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_font_collection_create(IntPtr fontManager, IntPtr typefaceProvider);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_font_collection_unref(IntPtr fontCollection);
    }
}