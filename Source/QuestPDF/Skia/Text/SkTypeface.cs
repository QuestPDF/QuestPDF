using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

public sealed class SkTypeface : IDisposable
{
    public IntPtr Instance { get; private set; }

    public SkTypeface(IntPtr instance)
    {
        Instance = instance;
    }
    
    ~SkTypeface()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.typeface_unref(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void typeface_unref(IntPtr typeface);
    }
}