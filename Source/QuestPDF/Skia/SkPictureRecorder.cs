using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkPictureRecorder : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkPictureRecorder()
    {
        Instance = API.picture_recorder_create();
        SkiaAPI.EnsureNotNull(Instance);
    }

    public SkCanvas BeginRecording(float width, float height)
    {
        var canvasInstance = API.picture_recorder_begin_recording(Instance, width, height);
        return new SkCanvas(canvasInstance, disposeNativeObject: false);
    }
    
    public SkPicture EndRecording()
    {
        var pictureInstance = API.picture_recorder_end_recording(Instance);
        return new SkPicture(pictureInstance);
    }
    
    ~SkPictureRecorder()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.picture_recorder_delete(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr picture_recorder_create();
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr picture_recorder_begin_recording(IntPtr pictureRecorder, float width, float height);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr picture_recorder_end_recording(IntPtr pictureRecorder);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void picture_recorder_delete(IntPtr pictureRecorder);
    }
}