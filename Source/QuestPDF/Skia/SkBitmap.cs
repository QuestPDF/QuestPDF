using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkBitmap : IDisposable
{
    public IntPtr Instance { get; private set; }

    public SkBitmap(int width, int height)
    {
        Instance = API.bitmap_create(width, height);
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public SkData EncodeAsJpeg(int quality)
    {
        var dataInstance = API.bitmap_encode_as_jpg(Instance, quality);
        return new SkData(dataInstance);
    }
    
    public SkData EncodeAsPng()
    {
        var dataInstance = API.bitmap_encode_as_png(Instance);
        return new SkData(dataInstance);
    }
    
    public SkData EncodeAsWebp(int quality)
    {
        var dataInstance = API.bitmap_encode_as_webp(Instance, quality);
        return new SkData(dataInstance);
    }
    
    ~SkBitmap()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.bitmap_delete(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bitmap_create(int width, int height);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void bitmap_delete(IntPtr image);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bitmap_encode_as_jpg(IntPtr image, int quality);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bitmap_encode_as_png(IntPtr image);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr bitmap_encode_as_webp(IntPtr image, int quality);
    }
}