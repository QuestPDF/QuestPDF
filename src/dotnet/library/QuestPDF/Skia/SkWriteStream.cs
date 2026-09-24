using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkWriteStream : IDisposable
{
    public IntPtr Instance { get; private set; }
    private GCHandle CallbackHandle { get; }
    
    private Stream TargetStream { get; }
    private ExceptionDispatchInfo? WriteException { get; set; }

    public SkWriteStream(Stream stream)
    {
        TargetStream = stream;

        var nativeCallback = new API.ByteArrayCallback(WriteToTargetStream);
        CallbackHandle = GCHandle.Alloc(nativeCallback);

        Instance = API.questpdf_skia_write_stream_create(nativeCallback);
        SkiaAPI.EnsureNotNull(Instance);
    }

    private void WriteToTargetStream(IntPtr data, int size)
    {
        if (WriteException != null)
            return;

        try
        {
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            unsafe
            {
                var span = new ReadOnlySpan<byte>((void*)data, size);
                TargetStream.Write(span);
            }
#else
            var managedArray = new byte[size];
            Marshal.Copy(data, managedArray, 0, size);
            TargetStream.Write(managedArray, 0, managedArray.Length);
#endif
        }
        catch (Exception exception)
        {
            WriteException = ExceptionDispatchInfo.Capture(exception);
        }
    }
    
    public void Flush()
    {
        API.questpdf_skia_write_stream_flush(Instance);
        ThrowIfWriteFailed();
    }
    
    public void ThrowIfWriteFailed()
    {
        WriteException?.Throw();
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
     
        API.questpdf_skia_write_stream_delete(Instance);
        Instance = IntPtr.Zero;
        CallbackHandle.Free();
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ByteArrayCallback(IntPtr data, int size);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_write_stream_create(ByteArrayCallback callback);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_write_stream_flush(IntPtr stream);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_write_stream_delete(IntPtr stream);
    }
}