using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;


[StructLayout(LayoutKind.Sequential)]
internal struct SkPlaceholderStyle
{
    public float Width;
    public float Height;
    public PlaceholderAlignment Alignment;
    public PlaceholderBaseline Baseline;
    public float BaselineOffset;

    public SkPlaceholderStyle()
    {
        Width = 0;
        Height = 0;
        Alignment = PlaceholderAlignment.AboveBaseline;
        Baseline = PlaceholderBaseline.Alphabetic;
        BaselineOffset = 0;
    }
    
    internal enum PlaceholderAlignment
    {
        /// Match the baseline of the placeholder with the baseline.
        Baseline,

        /// Align the bottom edge of the placeholder with the baseline such that the
        /// placeholder sits on top of the baseline.
        AboveBaseline,

        /// Align the top edge of the placeholder with the baseline specified in
        /// such that the placeholder hangs below the baseline.
        BelowBaseline,

        /// Align the top edge of the placeholder with the top edge of the font.
        /// When the placeholder is very tall, the extra space will hang from
        /// the top and extend through the bottom of the line.
        Top,

        /// Align the bottom edge of the placeholder with the top edge of the font.
        /// When the placeholder is very tall, the extra space will rise from
        /// the bottom and extend through the top of the line.
        Bottom,

        /// Align the middle of the placeholder with the middle of the text. When the
        /// placeholder is very tall, the extra space will grow equally from
        /// the top and bottom of the line.
        Middle,
    }
    
    internal enum PlaceholderBaseline
    {
        Alphabetic,
        Ideographic
    }
}

internal class SkParagraphBuilder : IDisposable
{
    internal IntPtr Instance;
    
    private SkParagraphBuilder(IntPtr instance)
    {
        Instance = instance;
    }
    
    public static SkParagraphBuilder Create(SkParagraphStyle paragraphStyle, SkFontCollection fontCollection)
    {
        var instance = API.paragraph_builder_create(paragraphStyle.Instance, fontCollection.Instance);
        return new SkParagraphBuilder(instance);
    }
    
    public void AddText(string text, SkTextStyle textStyle)
    {
        API.paragraph_builder_add_text(Instance, text, textStyle.Instance);
    }
    
    public void AddPlaceholder(SkPlaceholderStyle placeholderStyle)
    {
        API.paragraph_builder_add_placeholder(Instance, placeholderStyle);
    }
    
    public SkParagraph CreateParagraph()
    {
        var instance = API.paragraph_builder_create_paragraph(Instance);
        return new SkParagraph(instance);
    }
    
    ~SkParagraphBuilder()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.paragraph_builder_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr paragraph_builder_create(IntPtr paragraphStyle, IntPtr fontCollection);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_builder_add_text(IntPtr paragraphBuilder, string text, IntPtr textStyle);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_builder_add_placeholder(IntPtr paragraphBuilder, SkPlaceholderStyle placeholderStyle);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr paragraph_builder_create_paragraph(IntPtr paragraphBuilder);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void paragraph_builder_delete(IntPtr paragraphBuilder);
    }
}