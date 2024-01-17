using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

[StructLayout(LayoutKind.Sequential)]
internal struct ParagraphStyleConfiguration
{
    public TextAlign Alignment;
    public TextDirection Direction;
    
    public int MaxLinesVisible;
    public string Ellipsis;

    internal enum TextAlign
    {
        Left,
        Right,
        Center,
        Justify,
        Start,
        End
    }
    
    internal enum TextDirection
    {
        Rtl,
        Ltr
    }
}

internal class SkParagraphStyle : IDisposable
{
    internal IntPtr Instance;
    
    public SkParagraphStyle(IntPtr instance)
    {
        Instance = instance;
    }
    
    public SkParagraphStyle(ParagraphStyleConfiguration paragraphStyleConfiguration)
    {
        Instance = API.paragraph_style_create(paragraphStyleConfiguration);
    }

    ~SkParagraphStyle()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.paragraph_style_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr paragraph_style_create(ParagraphStyleConfiguration paragraphStyleConfiguration);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_style_delete(IntPtr paragraphStyle);
    }
}