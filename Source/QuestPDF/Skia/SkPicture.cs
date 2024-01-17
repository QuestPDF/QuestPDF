using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal class SkPicture : IDisposable
{
    internal IntPtr Instance;
    
    public SkPicture(IntPtr instance)
    {
        Instance = instance;
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
        
        API.picture_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void picture_delete(IntPtr picture);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr picture_serialize(IntPtr picture);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr picture_deserialize(IntPtr data);
    }
}