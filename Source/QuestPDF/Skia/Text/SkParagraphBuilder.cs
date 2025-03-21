using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

[StructLayout(LayoutKind.Sequential)]
internal record struct ParagraphStyleConfiguration
{
    public TextAlign Alignment;
    public TextDirection Direction;
    public int MaxLinesVisible;
    public IntPtr LineClampEllipsis; // SKText

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

record ParagraphStyle
{
    public ParagraphStyleConfiguration.TextAlign Alignment { get; init; }
    public ParagraphStyleConfiguration.TextDirection Direction { get; init; }
    public int MaxLinesVisible { get; init; }
    public string LineClampEllipsis { get; init; }
}

internal sealed class SkParagraphBuilder : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public ParagraphStyle Style { get; private set; }
    private SkFontCollection FontCollection { get; set; }

    public static SkParagraphBuilder Create(ParagraphStyle style, SkFontCollection fontCollection)
    {
        using var clampLinesEllipsis = new SkText(style.LineClampEllipsis);

        var paragraphStyleConfiguration = new ParagraphStyleConfiguration
        {
            Alignment = style.Alignment,
            Direction = style.Direction,
            MaxLinesVisible = style.MaxLinesVisible,
            LineClampEllipsis = clampLinesEllipsis.Instance
        };
        
        var instance = API.paragraph_builder_create(paragraphStyleConfiguration, fontCollection.Instance);
        SkiaAPI.EnsureNotNull(instance);
        
        return new SkParagraphBuilder
        {
            Instance = instance,
            Style = style,
            FontCollection = fontCollection
        };
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
    
    public void Reset()
    {
        API.paragraph_builder_reset(Instance);
    }
    
    ~SkParagraphBuilder()
    {
        this.WarnThatFinalizerIsReached();
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        FontCollection?.Dispose();
        
        API.paragraph_builder_delete(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr paragraph_builder_create(ParagraphStyleConfiguration paragraphStyleConfiguration, IntPtr fontCollection);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_builder_add_text(IntPtr paragraphBuilder, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(Utf8StringMarshaller))] string text, IntPtr textStyle);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_builder_add_placeholder(IntPtr paragraphBuilder, SkPlaceholderStyle placeholderStyle);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr paragraph_builder_create_paragraph(IntPtr paragraphBuilder);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_builder_reset(IntPtr paragraphBuilder);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void paragraph_builder_delete(IntPtr paragraphBuilder);
    }
}