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
        
        var size = Marshal.SizeOf<SkSize>();
        
        for (var i = 0; i < arrayLength; i++)
        {
            var ptr = IntPtr.Add(array, i * size);
            managedArray[i] = Marshal.PtrToStructure<SkSize>(ptr);
        }

        return managedArray;
    }
    
    public int[] GetUnresolvedCodepoints()
    {
        API.paragraph_get_unresolved_codepoints(Instance, out var array, out var arrayLength);
        
        var managedArray = new int[arrayLength];
        Marshal.Copy(array, managedArray,  0, arrayLength);

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
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_plan_layout(IntPtr paragraph, float availableWidth);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_get_line_metrics(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_get_unresolved_codepoints(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_get_placeholder_positions(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_get_text_range_positions(IntPtr paragraph, int rangeStart, int rangeEnd, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_delete(IntPtr paragraph);
    }
}