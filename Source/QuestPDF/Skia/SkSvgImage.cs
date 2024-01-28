using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

internal sealed class SkSvgImage : IDisposable
{
    public IntPtr Instance { get; private set; }
    public SkRect ViewBox;
    
    public SkSvgImage(string svgString)
    {
        using var data = SkData.FromBinary(System.Text.Encoding.UTF8.GetBytes(svgString));
        data.SetAsOwnedByAnotherObject();
        
        Instance = API.svg_create(data.Instance);
        
        if (Instance == IntPtr.Zero)
            throw new Exception("Cannot decode the provided SVG image.");
        
        API.svg_get_viewbox(Instance, out ViewBox);
    }
    
    ~SkSvgImage()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.svg_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr svg_create(IntPtr data);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void svg_delete(IntPtr svg);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void svg_get_viewbox(IntPtr svg, out SkRect viewbox);
    }
}