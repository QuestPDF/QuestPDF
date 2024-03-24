using System;
using System.IO;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkData : IDisposable
{
    public IntPtr Instance { get; private set; }

    public SkData(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
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
    
    public byte[] ToBytes()
    {
        var content = API.data_get_bytes(Instance);
        byte[] result = new byte[content.length];
        Marshal.Copy(content.bytes, result, 0, content.length);

        return result;
    }
    
    ~SkData()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.data_unref(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr data_create_from_file([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string path);
    
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
        public static extern void data_unref(IntPtr data);
    }
}