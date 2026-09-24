using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkPicture : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkPicture(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }

    ~SkPicture()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.questpdf_skia_picture_unref(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_picture_unref(IntPtr picture);
    }
}