using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkPictureRecorder : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkPictureRecorder()
    {
        Instance = API.picture_recorder_create();
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
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr picture_recorder_create();
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr picture_recorder_begin_recording(IntPtr pictureRecorder, float width, float height);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr picture_recorder_end_recording(IntPtr pictureRecorder);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void picture_recorder_delete(IntPtr pictureRecorder);
    }
}