using System;
using System.Runtime.InteropServices;

namespace NativeSkia.Text;

[StructLayout(LayoutKind.Sequential)]
internal struct TextStyleConfiguration
{
    public float FontSize;
    public FontWeights FontWeight;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)] public string[] FontFamilies;
    
    public uint ForegroundColor;
    public uint BackgroundColor;
    
    public uint DecorationColor;
    public TextDecoration DecorationType;
    public TextDecorationMode DecorationMode;
    public TextDecorationStyle DecorationStyle;
    
    public float LetterSpacing;
    public float WordSpacing;
    
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

internal class SkTextStyle : IDisposable
{
    internal IntPtr Instance;
    
    public SkTextStyle(IntPtr instance)
    {
        Instance = instance;
    }
    
    public SkTextStyle(TextStyleConfiguration textStyleConfiguration)
    {
        Instance = API.text_style_create(textStyleConfiguration);
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
        [DllImport(SkiaAPI.LibraryName)]
        public static extern IntPtr text_style_create(TextStyleConfiguration textStyleConfiguration);
        
        [DllImport(SkiaAPI.LibraryName)]
        public static extern void text_style_delete(IntPtr textStyle);
    }
}