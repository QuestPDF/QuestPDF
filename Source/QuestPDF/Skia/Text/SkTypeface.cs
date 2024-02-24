using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

public sealed class SkTypeface : IDisposable
{
    public IntPtr Instance { get; private set; }
    private bool DisposeNativeObject = true;
    
    public SkTypeface(IntPtr instance, bool disposeNativeObject)
    {
        Instance = instance;
        DisposeNativeObject = disposeNativeObject;
    }
    
    ~SkTypeface()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        if (DisposeNativeObject)
            API.typeface_delete(Instance);
        
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void typeface_delete(IntPtr typeface);
    }
}