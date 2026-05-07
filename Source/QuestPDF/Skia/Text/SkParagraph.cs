using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

[StructLayout(LayoutKind.Sequential)]
internal struct SkLineExtent
{
    public float Top;
    public float Bottom;
}

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

    public (float width, float height) GetSize()
    {
        API.paragraph_get_size(Instance, out var totalWidth, out var totalHeight);
        return (totalWidth, totalHeight);
    }
    
    public SkLineExtent[] GetLineExtents()
    {
        API.paragraph_get_line_extents(Instance, out var array, out var arrayLength);

        var managedArray = new SkLineExtent[arrayLength];
        
        var size = Marshal.SizeOf<SkLineExtent>();
        
        for (var i = 0; i < arrayLength; i++)
        {
            var ptr = IntPtr.Add(array, i * size);
            managedArray[i] = Marshal.PtrToStructure<SkLineExtent>(ptr);
        }

        API.paragraph_delete_line_extents(array);
        return managedArray;
    }
    
    public int[] GetUnresolvedCodepoints()
    {
        API.paragraph_get_unresolved_codepoints(Instance, out var array, out var arrayLength);
        
        var managedArray = new int[arrayLength];
        Marshal.Copy(array, managedArray,  0, arrayLength);
        API.paragraph_delete_unresolved_codepoints(array);

        return managedArray;
    }
    
    public SkRect[] GetPlaceholderPositions()
    {
        API.paragraph_get_placeholder_positions(Instance, out var array, out var arrayLength);
        
        var managedArray = new SkRect[arrayLength];
        
        var size = Marshal.SizeOf<SkRect>();
        
        for (var i = 0; i < arrayLength; i++)
        {
            var ptr = IntPtr.Add(array, i * size);
            managedArray[i] = Marshal.PtrToStructure<SkRect>(ptr);
        }

        API.paragraph_delete_positions(array);
        return managedArray;
    }
    
    public SkRect[] GetTextRangePositions(int rangeStart, int rangeEnd)
    {
        API.paragraph_get_text_range_positions(Instance, rangeStart, rangeEnd, out var array, out var arrayLength);
        
        var managedArray = new SkRect[arrayLength];
        
        var size = Marshal.SizeOf<SkRect>();
        
        for (var i = 0; i < arrayLength; i++)
        {
            var ptr = IntPtr.Add(array, i * size);
            managedArray[i] = Marshal.PtrToStructure<SkRect>(ptr);
        }

        API.paragraph_delete_positions(array);
        return managedArray;
    }
    
    ~SkParagraph()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.paragraph_delete(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_plan_layout(IntPtr paragraph, float availableWidth);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_get_size(IntPtr paragraph, out float totalWidth, out float totalHeight);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_get_line_extents(IntPtr paragraph, out IntPtr lineExtentsArray, out int lineExtentsArrayLength);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_delete_line_extents(IntPtr array);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_get_unresolved_codepoints(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_delete_unresolved_codepoints(IntPtr array);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_get_placeholder_positions(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_get_text_range_positions(IntPtr paragraph, int rangeStart, int rangeEnd, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_delete_positions(IntPtr array);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_delete(IntPtr paragraph);
    }
}