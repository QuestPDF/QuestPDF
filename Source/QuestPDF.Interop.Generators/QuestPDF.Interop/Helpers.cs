using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Interop;

internal class Helpers
{
    public static IntPtr BoxHandle(object obj)
    {
        var gch = GCHandle.Alloc(obj, GCHandleType.Normal);
        return GCHandle.ToIntPtr(gch);
    }

    public static T UnboxHandle<T>(IntPtr handle) where T : class
    {
        var gch = GCHandle.FromIntPtr(handle);
        return (T)gch.Target!;
    }

    public static void FreeHandle(IntPtr handle)
    {
        if (handle == 0) 
            return;
        
        var gch = GCHandle.FromIntPtr(handle);
        
        if (gch.IsAllocated) 
            gch.Free();
    }
}