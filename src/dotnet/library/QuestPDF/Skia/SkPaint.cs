using System;
using System.Linq;
using System.Runtime.InteropServices;
using QuestPDF.Infrastructure;

namespace QuestPDF.Skia;

internal sealed class SkPaint : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public bool IsShared { get; private set; }

    public SkPaint()
    {
        Instance = API.questpdf_skia_paint_create();
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    public void SetSolidColor(uint color)
    {
        EnsureNotShared();
        API.questpdf_skia_paint_set_solid_color(Instance, color);
    }
    
    public void SetLinearGradient(Position start, Position end, Color[] colors)
    {
        if (colors.Length == 0)
            throw new ArgumentException("At least one color must be provided to create a gradient.", nameof(colors));
        
        EnsureNotShared();

        var startPoint = new SkPoint(start.X, start.Y);
        var endPoint = new SkPoint(end.X, end.Y);
        
        var colorArray = colors.Select(c => c.Hex).ToArray();
        
        API.questpdf_skia_paint_set_linear_gradient(Instance, in startPoint, in endPoint, colorArray.Length, colorArray);
    }
    
    public void SetStroke(float thickness)
    {
        EnsureNotShared();
        API.questpdf_skia_paint_set_stroke(Instance, thickness);
    }
    
    public void SetDashedPathEffect(float[] intervals)
    {
        if (intervals.Length == 0)
            throw new ArgumentException("At least one interval must be provided to create a dashed path effect.", nameof(intervals));
        
        if (intervals.Length % 2 != 0)
            throw new ArgumentException("The intervals array must contain an even number of elements.", nameof(intervals));

        EnsureNotShared();
        API.questpdf_skia_paint_set_dashed_path_effect(Instance, intervals.Length, intervals);
    }
    
    public void MarkAsShared()
    {
        IsShared = true;
        GC.SuppressFinalize(this);
    }

    private void EnsureNotShared()
    {
        if (IsShared)
            throw new InvalidOperationException("This SkPaint instance is shared via the SkPaintCache and cannot be modified.");
    }

    ~SkPaint()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }

    public void Dispose()
    {
        if (IsShared)
            return;

        if (Instance == IntPtr.Zero)
            return;
        
        API.questpdf_skia_paint_delete(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_paint_create();
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_paint_delete(IntPtr paint);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_paint_set_solid_color(IntPtr paint, uint color);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_paint_set_linear_gradient(IntPtr paint, in SkPoint start, in SkPoint end, int colorsLength, uint[] colors);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_paint_set_stroke(IntPtr paint, float thickness);    
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_paint_set_dashed_path_effect(IntPtr paint, int arrayLength, float[] intervals); 
    }
}