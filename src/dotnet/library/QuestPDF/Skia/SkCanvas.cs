using System;
using System.Runtime.InteropServices;
using QuestPDF.Skia.Text;

namespace QuestPDF.Skia;

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
        var instance = API.questpdf_skia_canvas_create_from_bitmap(bitmap.Instance);
        return new SkCanvas(instance);
    }
    
    public void Save()
    {
        API.questpdf_skia_canvas_save(Instance);
    }
    
    public void Restore()
    {
        API.questpdf_skia_canvas_restore(Instance);
    }
    
    public void Translate(float x, float y)
    {
        API.questpdf_skia_canvas_translate(Instance, x, y);
    }
    
    public void Scale(float factorX, float factorY)
    {
        API.questpdf_skia_canvas_scale(Instance, factorX, factorY);
    }
    
    public void Rotate(float degrees)
    {
        API.questpdf_skia_canvas_rotate(Instance, degrees);
    }
    
    public void DrawLine(SkPoint start, SkPoint end, SkPaint paint)
    {
        API.questpdf_skia_canvas_draw_line(Instance, in start, in end, paint.Instance);
    }

    public void DrawRectangle(SkRect position, SkPaint paint)
    {
        API.questpdf_skia_canvas_draw_rectangle(Instance, in position, paint.Instance);
    }
    
    public void DrawComplexBorder(SkRoundedRect innerRect, SkRoundedRect outerRect, SkPaint paint)
    {
        API.questpdf_skia_canvas_draw_complex_border(Instance, in innerRect, in outerRect, paint.Instance);
    }
    
    public void DrawShadow(SkRoundedRect shadowRect, SkBoxShadow shadow)
    {
        API.questpdf_skia_canvas_draw_shadow(Instance, in shadowRect, in shadow);
    }
    
    public void DrawImage(SkImage image, float width, float height)
    {
        API.questpdf_skia_canvas_draw_image(Instance, image.Instance, width, height);
    }
    
    public void DrawPicture(SkPicture picture)
    {
        API.questpdf_skia_canvas_draw_picture(Instance, picture.Instance);
    }
    
    public void DrawParagraph(SkParagraph paragraph, int? lineFrom = null, int? lineTo = null)
    {
        API.questpdf_skia_canvas_draw_paragraph(Instance, paragraph.Instance, lineFrom ?? 0, lineTo ?? int.MaxValue);
    }
    
    public void DrawSvgPath(string svg, uint color)
    {
        API.questpdf_skia_canvas_draw_svg_path(Instance, svg, color);
    }
    
    public void DrawSvg(SkSvgImage svgImage, float width, float height)
    {
        API.questpdf_skia_canvas_draw_svg(Instance, svgImage.Instance, width, height);
    }
    
    /// <summary>
    /// draws stripe pattern (red lines at 45 deegree angle)
    /// </summary>
    public void DrawOverflowArea(SkRect position)
    {
        API.questpdf_skia_canvas_draw_overflow_area(Instance, in position);
    }
    
    public void ClipOverflowArea(SkRect availableSpace, SkRect requiredSpace)
    {
        API.questpdf_skia_canvas_clip_overflow_area(Instance, in availableSpace, in requiredSpace);
    }
    
    public void ClipRectangle(SkRect clipArea)
    {
        API.questpdf_skia_canvas_clip_rectangle(Instance, in clipArea);
    }
    
    public void ClipRoundedRectangle(SkRoundedRect rect)
    {
        API.questpdf_skia_canvas_clip_rounded_rectangle(Instance, in rect);
    }
    
    public void AnnotateUrl(float width, float height, string url, string? description)
    {
        API.questpdf_skia_canvas_annotate_url(Instance, width, height, url, description);
    }
    
    public void AnnotateDestination(string destinationName)
    {
        API.questpdf_skia_canvas_annotate_destination(Instance, destinationName);
    }
    
    public void AnnotateDestinationLink(float width, float height, string destinationName, string? description)
    {
        API.questpdf_skia_canvas_annotate_destination_link(Instance, width, height, destinationName, description);
    }
    
    public SkCanvasMatrix GetCurrentMatrix()
    { 
        API.questpdf_skia_canvas_get_matrix9(Instance, out var matrix);
        return matrix;
    }
    
    public void SetCurrentMatrix(SkCanvasMatrix matrix)
    {
        API.questpdf_skia_canvas_set_matrix9(Instance, in matrix);
    }
    
    public void SetSemanticNodeId(int nodeId)
    {
        API.questpdf_skia_canvas_set_semantic_node_id(Instance, nodeId);
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
            API.questpdf_skia_canvas_delete(Instance);
        
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_canvas_create_from_bitmap(IntPtr bitmap);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_delete(IntPtr canvas);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_save(IntPtr canvas);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_restore(IntPtr canvas);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_translate(IntPtr canvas, float x, float y);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_rotate(IntPtr canvas, float angle);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_scale(IntPtr canvas, float factorX, float factorY);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_draw_image(IntPtr canvas, IntPtr image, float width, float height);
    
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_draw_picture(IntPtr canvas, IntPtr picture);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_draw_line(IntPtr canvas, in SkPoint start, in SkPoint end, IntPtr paint);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_draw_rectangle(IntPtr canvas, in SkRect position, IntPtr paint);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_draw_complex_border(IntPtr canvas, in SkRoundedRect innerRect, in SkRoundedRect outerRect, IntPtr paint);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_draw_shadow(IntPtr canvas, in SkRoundedRect shadowRect, in SkBoxShadow shadow);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_draw_paragraph(IntPtr canvas, IntPtr paragraph, int lineFrom, int lineTo);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_draw_svg_path(IntPtr canvas, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string svg, uint color);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_draw_svg(IntPtr canvas, IntPtr svg, float width, float height);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_draw_overflow_area(IntPtr canvas, in SkRect position);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_clip_overflow_area(IntPtr canvas, in SkRect availableSpace, in SkRect requiredSpace);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_clip_rectangle(IntPtr canvas, in SkRect clipArea);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_clip_rounded_rectangle(IntPtr canvas, in SkRoundedRect rect);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_annotate_url(
            IntPtr canvas, 
            float width, 
            float height, 
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string url,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string? description);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_annotate_destination(IntPtr canvas, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string destinationName);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_annotate_destination_link(
            IntPtr canvas, 
            float width,
            float height, 
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string destinationName,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string? description);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_get_matrix9(IntPtr canvas, out SkCanvasMatrix matrix);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_set_matrix9(IntPtr canvas, in SkCanvasMatrix matrix);

        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_canvas_set_semantic_node_id(IntPtr canvas, int nodeId);
    }
}