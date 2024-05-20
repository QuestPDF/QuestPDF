using System;
using System.Runtime.InteropServices;

namespace QuestPDF.Skia.Text;

[StructLayout(LayoutKind.Sequential)]
internal struct TextStyleConfiguration
{
    public float FontSize;
    public FontWeights FontWeight;
    public bool IsItalic;

    public const int FONT_FAMILIES_LENGTH = 16;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = FONT_FAMILIES_LENGTH)] public IntPtr[] FontFamilies;
    
    public uint ForegroundColor;
    public uint BackgroundColor;
    
    public uint DecorationColor;
    public float DecorationThickness;
    public TextDecoration DecorationType;
    public TextDecorationMode DecorationMode;
    public TextDecorationStyle DecorationStyle;
    
    public float LineHeight; // when 0, the default font metrics are used 
    public float LetterSpacing;
    public float WordSpacing;
    public float BaselineOffset;
    
    public enum FontWeights
    {
        Invisible = 0,
        Thin = 100,
        ExtraLight = 200,
        Light = 300,
        Normal = 400,
        Medium = 500,
        SemiBold = 600,
        Bold = 700,
        ExtraBold = 800,
        Black = 900,
        ExtraBlack = 1000,
    }
    
    [Flags]
    public enum TextDecoration
    {
        NoDecoration = 0x0,
        Underline = 0x1,
        Overline = 0x2,
        LineThrough = 0x4
    }
    
    public enum TextDecorationMode
    {
        Gaps, 
        Through
    }
    
    public enum TextDecorationStyle
    {
        Solid, 
        Double, 
        Dotted, 
        Dashed, 
        Wavy
    }
}

internal sealed class SkTextStyle : IDisposable
{
    public IntPtr Instance { get; private set; }
    
    public SkTextStyle(TextStyleConfiguration textStyleConfiguration)
    {
        Instance = API.text_style_create(textStyleConfiguration);
        SkiaAPI.EnsureNotNull(Instance);
    }
    
    ~SkTextStyle()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (Instance == IntPtr.Zero)
            return;
        
        API.text_style_delete(Instance);
        Instance = IntPtr.Zero;
    }
    
    private static class API
    {
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr text_style_create(TextStyleConfiguration textStyleConfiguration);
        
        [DllImport(SkiaAPI.LibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void text_style_delete(IntPtr textStyle);
    }
}