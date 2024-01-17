using System;
using System.Runtime.InteropServices;

namespace NativeSkia;

internal class SkBitmap : IDisposable
{
    internal IntPtr Instance;
    
    public SkBitmap(IntPtr instance)
    {
        Instance = instance;
    }
    
    public SkBitmap(int width, int height)
    {
        Instance = API.bitmap_create(width, height);
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
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.bitmap_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr bitmap_create(int width, int height);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void bitmap_delete(IntPtr image);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr bitmap_encode_as_jpg(IntPtr image, int quality);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr bitmap_encode_as_png(IntPtr image);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr bitmap_encode_as_webp(IntPtr image, int quality);
    }
}