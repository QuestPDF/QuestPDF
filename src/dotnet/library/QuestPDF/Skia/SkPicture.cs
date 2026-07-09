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
    
    public static SkPicture Deserialize(SkData data)
    {
        var instance = API.questpdf_skia_picture_deserialize(data.Instance);
        return new SkPicture(instance);
    }
    
    public SkData Serialize()
    {
        var dataInstance = API.questpdf_skia_picture_serialize(Instance);
        return new SkData(dataInstance);
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
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_picture_serialize(IntPtr picture);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_picture_deserialize(IntPtr data);
    }
}