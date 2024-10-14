using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

internal sealed class SkParagraph : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkParagraph(IntPtr instance)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
    }

    public void PlanLayout(float availableWidth)
    {
        API.paragraph_plan_layout(Instance, availableWidth);
    }
    
    public SkSize[] GetLineMetrics()
    {
        API.paragraph_get_line_metrics(Instance, out var array, out var arrayLength);
        
        var managedArray = new SkSize[arrayLength];

#if NET45
        var size = Marshal.SizeOf(typeof(SkSize));
#else
        var size = Marshal.SizeOf<SkSize>();
#endif
        
        for (var i = 0; i < arrayLength; i++)
        {
            var ptr = IntPtr.Add(array, i * size);

#if NET45
            managedArray[i] = (SkSize)Marshal.PtrToStructure(ptr, typeof(SkSize));
#else
            managedArray[i] = Marshal.PtrToStructure<SkSize>(ptr);
#endif
        }

        Marshal.FreeHGlobal(array);
        return managedArray;
    }
    
    public int[] GetUnresolvedCodepoints()
    {
        API.paragraph_get_unresolved_codepoints(Instance, out var array, out var arrayLength);
        
        var managedArray = new int[arrayLength];
        Marshal.Copy(array, managedArray,  0, arrayLength);
        Marshal.FreeHGlobal(array);

        return managedArray;
    }
    
    public SkRect[] GetPlaceholderPositions()
    {
        API.paragraph_get_placeholder_positions(Instance, out var array, out var arrayLength);
        
        var managedArray = new SkRect[arrayLength];

#if NET45
        var size = Marshal.SizeOf(typeof(SkRect));
#else
        var size = Marshal.SizeOf<SkRect>();
#endif
        
        for (var i = 0; i < arrayLength; i++)
        {
            var ptr = IntPtr.Add(array, i * size);
#if NET45
            managedArray[i] = (SkRect)Marshal.PtrToStructure(ptr, typeof(SkRect));
#else
            managedArray[i] = Marshal.PtrToStructure<SkRect>(ptr);
#endif
        }

        Marshal.FreeHGlobal(array);
        return managedArray;
    }
    
    public SkRect[] GetTextRangePositions(int rangeStart, int rangeEnd)
    {
        API.paragraph_get_text_range_positions(Instance, rangeStart, rangeEnd, out var array, out var arrayLength);
        
        var managedArray = new SkRect[arrayLength];

#if NET45
        var size = Marshal.SizeOf(typeof(SkRect));
#else
        var size = Marshal.SizeOf<SkRect>();
#endif
        
        for (var i = 0; i < arrayLength; i++)
        {
            var ptr = IntPtr.Add(array, i * size);

#if NET45
            managedArray[i] = (SkRect)Marshal.PtrToStructure(ptr, typeof(SkRect));
#else
            managedArray[i] = Marshal.PtrToStructure<SkRect>(ptr);
#endif
        }

        Marshal.FreeHGlobal(array);
        return managedArray;
    }
    
    ~SkParagraph()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.paragraph_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_plan_layout(IntPtr paragraph, float availableWidth);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_get_line_metrics(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_get_unresolved_codepoints(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_get_placeholder_positions(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_get_text_range_positions(IntPtr paragraph, int rangeStart, int rangeEnd, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_delete(IntPtr paragraph);
    }
}