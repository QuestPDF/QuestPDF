using System;
using System.Text;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal class SkText : IDisposable
{
    public IntPtr Instance { get; private set; }

    public SkText(string? text)
    {
        Instance = MarshalFromManagedToNative(text);
        SkiaAPI.EnsureNotNull(Instance);
    }

    ~SkText()
    {
        Dispose();
    }
    
    public static unsafe IntPtr MarshalFromManagedToNative(string? text)
    {
        if (text == null) 
            return IntPtr.Zero;

        var length = Encoding.UTF8.GetByteCount(text);
        var nativeArray = Marshal.AllocHGlobal(length + 1);
        
        fixed (char* pText = text)
        {
            var ptr = (byte*)nativeArray;
            Encoding.UTF8.GetBytes(pText, text.Length, ptr, length);
            *(ptr + length) = 0; // null termination
        }
        
        return nativeArray;
    } 
    
    public static implicit operator IntPtr(SkText text) => text.Instance;
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        Marshal.FreeHGlobal(Instance);
        Instance = IntPtr.Zero;
    }
}