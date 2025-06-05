using System;
using System.IO;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkWriteStream : IDisposable
{
    public IntPtr Instance { get; private set; }
    private GCHandle CallbackHandle { get; }

    public SkWriteStream(Stream stream)
    {
        var nativeCallback = new API.ByteArrayCallback((data, size) =>
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            unsafe
            {
                var span = new ReadOnlySpan<byte>((void*)data, size);
                stream.Write(span);
            }
#else
            var managedArray = new byte[size];
            Marshal.Copy(data, managedArray, 0, size);
            stream?.Write(managedArray, 0, managedArray.Length);
#endif
        });

        CallbackHandle = GCHandle.Alloc(nativeCallback);
        
        Instance = API.write_stream_create(nativeCallback);
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public void Flush()
    {
        API.write_stream_flush(Instance);
    }
    
    ~SkWriteStream()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
     
        CallbackHandle.Free();
        API.write_stream_delete(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ByteArrayCallback(IntPtr data, int size);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr write_stream_create(ByteArrayCallback callback);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void write_stream_flush(IntPtr stream);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void write_stream_delete(IntPtr stream);
    }
}