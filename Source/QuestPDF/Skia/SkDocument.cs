using System;
using System.Runtime.InteropServices;

namespace NativeSkia;

internal class SkDocument : IDisposable
{
    internal IntPtr Instance;

    internal SkDocument(IntPtr instance)
    {
        Instance = instance;
    }

    public SkCanvas BeginPage(float width, float height)
    {
        var instance = API.document_begin_page(Instance, width, height);
        return new SkCanvas(instance, false);
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
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.document_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr document_begin_page(IntPtr document, float width, float height);
    
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void document_end_page(IntPtr document);
    
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void document_close(IntPtr document);
    
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void document_delete(IntPtr document);
    }
}