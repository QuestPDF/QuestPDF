using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

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

internal readonly record struct ParagraphStyle
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

    private bool IsPooled { get; set; }

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
        
        var instance = API.questpdf_skia_paragraph_builder_create(in paragraphStyleConfiguration, SkUnicode.Global.Instance, fontCollection.Instance);
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
        if (string.IsNullOrEmpty(text))
            return;

        AddText(text.AsSpan(), textStyle);
    }

    [SkipLocalsInit]
    public unsafe void AddText(ReadOnlySpan<char> text, SkTextStyle textStyle)
    {
        if (text.IsEmpty)
            return;

        fixed (char* textPointer = text)
        {
            var utf8Length = Encoding.UTF8.GetByteCount(textPointer, text.Length);

            var nativeText = Marshal.AllocHGlobal(utf8Length + 1);

            var nativeTextPointer = (byte*)nativeText;
            Encoding.UTF8.GetBytes(textPointer, text.Length, nativeTextPointer, utf8Length);
            nativeTextPointer[utf8Length] = 0; // null termination

            API.questpdf_skia_paragraph_builder_add_text(Instance, nativeText, textStyle.Instance);
            Marshal.FreeHGlobal(nativeText);
        }
    }
    
    public void AddPlaceholder(SkPlaceholderStyle placeholderStyle)
    {
        API.questpdf_skia_paragraph_builder_add_placeholder(Instance, in placeholderStyle);
    }
    
    public SkParagraph CreateParagraph()
    {
        var instance = API.questpdf_skia_paragraph_builder_create_paragraph(Instance);
        return new SkParagraph(instance);
    }
    
    public void Reset()
    {
        API.questpdf_skia_paragraph_builder_reset(Instance);
    }

    /// <summary>
    /// Marks the builder as owned by <see cref="QuestPDF.Elements.Text.SkParagraphBuilderPoolManager"/>.
    /// Pooled builders are reused by every document rendered on their thread, so no scope disposes them;
    /// they are released by the finalizer once the owning thread is gone, which is expected and not a leak.
    /// </summary>
    public void MarkAsPooled()
    {
        IsPooled = true;
        FontCollection.MarkAsPooled();
    }
    
    ~SkParagraphBuilder()
    {
        if (!IsPooled)
            this.WarnThatFinalizerIsReached();

        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        FontCollection?.Dispose();
        
        API.questpdf_skia_paragraph_builder_delete(Instance);
        Instance = IntPtr.Zero;
        GC.SuppressFinalize(this);
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_paragraph_builder_create(in ParagraphStyleConfiguration paragraphStyleConfiguration, IntPtr unicode, IntPtr fontCollection);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_paragraph_builder_add_text(IntPtr paragraphBuilder, IntPtr textPointer, IntPtr textStyle);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_paragraph_builder_add_placeholder(IntPtr paragraphBuilder, in SkPlaceholderStyle placeholderStyle);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr questpdf_skia_paragraph_builder_create_paragraph(IntPtr paragraphBuilder);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_paragraph_builder_reset(IntPtr paragraphBuilder);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void questpdf_skia_paragraph_builder_delete(IntPtr paragraphBuilder);
    }
}