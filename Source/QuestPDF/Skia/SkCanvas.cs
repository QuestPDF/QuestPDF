using System;
using System.Runtime.InteropServices;
using QuestPDF.Skia.Text;

namespace QuestPDF.Skia;

[StructLayout(LayoutKind.Sequential)]
internal struct SkCanvasMatrix
{
    public float ScaleX;
    public float SkewX;
    public float TranslateX;
    
    public float SkewY;
    public float ScaleY;
    public float TranslateY;

    public float Perspective1;
    public float Perspective2;
    public float Perspective3;
}

internal sealed class SkCanvas : IDisposable
{
    public IntPtr Instance { get; private set; }
    private bool DisposeNativeObject { get; }

    public SkCanvas(IntPtr instance, bool disposeNativeObject = true)
    {
        Instance = instance;
        SkiaAPI.EnsureNotNull(Instance);
        
        DisposeNativeObject = disposeNativeObject;
    }
    
    public static SkCanvas CreateFromBitmap(SkBitmap bitmap)
    {
        var instance = API.canvas_create_from_bitmap(bitmap.Instance);
        return new SkCanvas(instance);
    }
    
    public void Save()
    {
        API.canvas_save(Instance);
    }
    
    public void Restore()
    {
        API.canvas_restore(Instance);
    }
    
    public void Translate(float x, float y)
    {
        API.canvas_translate(Instance, x, y);
    }
    
    public void Scale(float factorX, float factorY)
    {
        API.canvas_scale(Instance, factorX, factorY);
    }
    
    public void Rotate(float degrees)
    {
        API.canvas_rotate(Instance, degrees);
    }
    
    public void DrawFilledRectangle(SkRect position, uint color)
    {
        API.canvas_draw_filled_rectangle(Instance, position, color);
    }
    
    public void DrawStrokeRectangle(SkRect position, float strokeWidth, uint strokeColor)
    {
        API.canvas_draw_stroke_rectangle(Instance, position, strokeWidth, strokeColor);
    }
    
    public void DrawImage(SkImage image, float width, float height)
    {
        API.canvas_draw_image(Instance, image.Instance, width, height);
    }
    
    public void DrawPicture(SkPicture picture)
    {
        API.canvas_draw_picture(Instance, picture.Instance);
    }
    
    public void DrawParagraph(SkParagraph paragraph, int? lineFrom = null, int? lineTo = null)
    {
        API.canvas_draw_paragraph(Instance, paragraph.Instance, lineFrom ?? 0, lineTo ?? int.MaxValue);
    }
    
    public void DrawSvgPath(string svg, uint color)
    {
        API.canvas_draw_svg_path(Instance, svg, color);
    }
    
    public void DrawSvg(SkSvgImage svgImage, float width, float height)
    {
        API.canvas_draw_svg(Instance, svgImage.Instance, width, height);
    }
    
    /// <summary>
    /// draws stripe pattern (red lines at 45 deegree angle)
    /// </summary>
    public void DrawOverflowArea(SkRect position)
    {
        API.canvas_draw_overflow_area(Instance, position);
    }
    
    public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace)
    {
        API.canvas_clip_overflow_area(Instance, availableSpace, requiredSpace);
    }
    
    public void ClipRectangle(SkRect clipArea)
    {
        API.canvas_clip_rectangle(Instance, clipArea);
    }
    
    public void AnnotateUrl(float width, float height, string url)
    {
        API.canvas_annotate_url(Instance, width, height, url);
    }
    
    public void AnnotateDestination(string destinationName)
    {
        API.canvas_annotate_destination(Instance, destinationName);
    }
    
    public void AnnotateDestinationLink(float width, float height, string destinationName)
    {
        API.canvas_annotate_destination_link(Instance, width, height, destinationName);
    }
    
    public SkCanvasMatrix GetCurrentMatrix()
    {
        API.canvas_get_matrix9(Instance, out var result);
        return result;
    }
    
    public void SetCurrentMatrix(SkCanvasMatrix matrix)
    {
        API.canvas_set_matrix9(Instance, matrix);
    }
    
    ~SkCanvas()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        if (DisposeNativeObject)
            API.canvas_delete(Instance);
        
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr canvas_create_from_bitmap(IntPtr bitmap);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_delete(IntPtr canvas);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_save(IntPtr canvas);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_restore(IntPtr canvas);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_translate(IntPtr canvas, float x, float y);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_rotate(IntPtr canvas, float angle);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_scale(IntPtr canvas, float factorX, float factorY);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_draw_image(IntPtr canvas, IntPtr image, float width, float height);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_draw_picture(IntPtr canvas, IntPtr picture);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_draw_filled_rectangle(IntPtr canvas, SkRect position, uint color);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_draw_stroke_rectangle(IntPtr canvas, SkRect position, float strokeWidth, uint color);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_draw_paragraph(IntPtr canvas, IntPtr paragraph, int lineFrom, int lineTo);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_draw_svg_path(IntPtr canvas, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string svg, uint color);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_draw_svg(IntPtr canvas, IntPtr svg, float width, float height);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_draw_overflow_area(IntPtr canvas, SkRect position);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_clip_overflow_area(IntPtr canvas, SkRect availableSpace, SkRect requiredSpace);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_clip_rectangle(IntPtr canvas, SkRect clipArea);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_annotate_url(IntPtr canvas, float width, float height, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string url);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_annotate_destination(IntPtr canvas, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string destinationName);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_annotate_destination_link(IntPtr canvas, float width, float height, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string destinationName);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_get_matrix9(IntPtr canvas, out SkCanvasMatrix matrix);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void canvas_set_matrix9(IntPtr canvas, SkCanvasMatrix matrix);
    }
}