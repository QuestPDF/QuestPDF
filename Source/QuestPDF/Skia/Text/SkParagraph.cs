using System;
using System.Runtime.InteropServices;

namespace NativeSkia.Text;

internal class SkParagraph : IDisposable
{
    internal IntPtr Instance;
    
    public SkParagraph(IntPtr instance)
    {
        Instance = instance;
    }

    public void PlanLayout(float availableWidth)
    {
        API.paragraph_plan_layout(Instance, availableWidth);
    }
    
    public double[] GetLineHeights()
    {
        API.paragraph_get_line_heights(Instance, out var array, out var arrayLength);
        
        var managedArray = new double[arrayLength];
        Marshal.Copy(array, managedArray,  0, arrayLength);

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
        public static extern void paragraph_get_line_heights(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_get_unresolved_codepoints(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_get_placeholder_positions(IntPtr paragraph, out IntPtr array, out int arrayLength);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_delete(IntPtr paragraph);
    }
}