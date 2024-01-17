using System;
using System.IO;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal class SkData : IDisposable
{
    internal IntPtr Instance;
    internal bool DisposeNativeObject = true;
    
    public SkData(IntPtr instance, bool disposeNativeObject = true)
    {
        Instance = instance;
        DisposeNativeObject = disposeNativeObject;
    }

    public void SetAsOwnedByAnotherObject()
    {
        DisposeNativeObject = false;
    }
    
    public static SkData FromFile(string filePath)
    {
        var instance = API.data_create_from_file(filePath);
        return new SkData(instance);
    }
    
    public static SkData FromStream(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        var binaryData = memoryStream.ToArray();
        
        return SkData.FromBinary(binaryData);
    }
    
    public static unsafe SkData FromBinary(byte[] data)
    {
        fixed (byte* dataPtr = data)
        {
            var instance = API.data_create_from_binary(dataPtr, data.Length);
            GC.KeepAlive(data);
            return new SkData(instance);
        }
    }
    
    public unsafe ReadOnlySpan<byte> ToSpan()
    {
        var content = API.data_get_bytes(Instance);
        return new ReadOnlySpan<byte> (content.bytes.ToPointer(), content.length);
    }

    ~SkData()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        if (DisposeNativeObject)
            API.data_delete(Instance);
        
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr data_create_from_file(string path);
    
        [DllImport(SkiaAPI.LibraryName)]
        public static extern unsafe IntPtr data_create_from_binary(byte* arrayPointer, int arrayLength);
    
        [StructLayout(LayoutKind.Sequential)]
        public struct GetBytesFromDataResult
        {
            public int length;
            public IntPtr bytes;
        }
    
        [DllImport(SkiaAPI.LibraryName)]
        public static extern GetBytesFromDataResult data_get_bytes(IntPtr data);
    
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void data_delete(IntPtr data);
    }
}