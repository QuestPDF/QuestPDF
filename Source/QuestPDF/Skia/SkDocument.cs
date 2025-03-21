using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkDocument : IDisposable
{
    public IntPtr Instance { get; private set; }

    internal SkDocument(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }

    public SkCanvas BeginPage(float width, float height)
    {
        var instance = API.document_begin_page(Instance, width, height);
        return new SkCanvas(instance, disposeNativeObject: false);
    }
    
    public void EndPage()
    {
        API.document_end_page(Instance);
    }

    public void Close()
    {
        API.document_close(Instance);
    }
    
    ~SkDocument()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.document_unref(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr document_begin_page(IntPtr document, float width, float height);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void document_end_page(IntPtr document);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void document_close(IntPtr document);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void document_unref(IntPtr document);
    }
}