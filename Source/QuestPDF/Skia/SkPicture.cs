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
        var instance = API.picture_deserialize(data.Instance);
        return new SkPicture(instance);
    }
    
    public SkData Serialize()
    {
        var dataInstance = API.picture_serialize(Instance);
        return new SkData(dataInstance);
    }
    
    ~SkPicture()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.picture_unref(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void picture_unref(IntPtr picture);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr picture_serialize(IntPtr picture);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr picture_deserialize(IntPtr data);
    }
}