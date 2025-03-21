using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkImage : IDisposable
{
    public IntPtr Instance { get; private set; }

    public readonly int Width;
    public readonly int Height;
    public readonly int EncodedDataSize;
    
    public SkImage(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
        
        // load image details
        var details = API.image_get_details(Instance);
        
        Width = details.Width;
        Height = details.Height;
        EncodedDataSize = details.EncodedDataSize;
        
        GC.AddMemoryPressure(EncodedDataSize);
    }

    public static SkImage FromData(SkData data)
    {
        var instance = API.image_create_from_data(data.Instance);
        
        if (instance == IntPtr.Zero)
            throw new Exception("Cannot decode the provided image.");
        
        return new SkImage(instance);
    }
    
    /// <summary>
    /// Scales image only when target size is smaller than original image size.
    /// When image is opaque, uses the JPEG compression algorithm, otherwise uses the PNG algorithm.
    /// Only the JPEG compression algorithm uses the compressionQuality parameter.
    /// </summary>
    public SkImage ResizeAndCompress(int targetWidth, int targetHeight, int compressionQuality, bool downsample)
    {
        var instance = API.image_resize_and_compress(Instance, targetWidth, targetHeight, compressionQuality, downsample);
        return new SkImage(instance);
    }
    
    public static SkImage GeneratePlaceholder(int targetWidth, int targetHeight, uint firstColor, uint secondColor)
    {
        var instance = API.image_generate_placeholder(targetWidth, targetHeight, firstColor, secondColor);
        return new SkImage(instance);
    }
    
    public SkData GetEncodedData()
    {
        var dataInstance = API.image_get_encoded_data(Instance);
        return new SkData(dataInstance);
    }
    
    ~SkImage()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.image_unref(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
        GC.RemoveMemoryPressure(EncodedDataSize);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr image_create_from_data(IntPtr data);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void image_unref(IntPtr image);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr image_resize_and_compress(IntPtr image, int targetImageWidth, int targetImageHeight, int compressionQuality, bool downsample);

        [StructLayout(LayoutKind.Sequential)]
        public struct SkImageDetails
        {
            public int Width;
            public int Height;
            public int EncodedDataSize;
        }

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern SkImageDetails image_get_details(IntPtr image);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr image_get_encoded_data(IntPtr image);
 
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr image_generate_placeholder(int imageWidth, int imageHeight, UInt32 firstColor, UInt32 secondColor);
    }
}