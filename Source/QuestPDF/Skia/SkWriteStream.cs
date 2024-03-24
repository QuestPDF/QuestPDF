using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkWriteStream : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkWriteStream()
    {
        Instance = API.write_stream_create();
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public SkData DetachData()
    {
        var dataInstance = API.write_stream_detach_data(Instance);
        return new SkData(dataInstance);
    }
    
    ~SkWriteStream()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.write_stream_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr write_stream_create();
    
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void write_stream_delete(IntPtr stream);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr write_stream_detach_data(IntPtr stream);    
    }
}