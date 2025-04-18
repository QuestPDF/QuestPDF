using System;
using System.Runtime.InteropServices;

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

    public static SkCanvasMatrix Identity => new()
    {
        ScaleX = 1,
        SkewX = 0,
        TranslateX = 0,
        
        SkewY = 0,
        ScaleY = 1,
        TranslateY = 0,
        
        Perspective1 = 0,
        Perspective2 = 0,
        Perspective3 = 1
    };
    
    public static SkCanvasMatrix CreateTranslation(float x, float y)
    {
        return new SkCanvasMatrix
        {
            ScaleX = 1,
            SkewX = 0,
            TranslateX = x,
            
            SkewY = 0,
            ScaleY = 1,
            TranslateY = y,
            
            Perspective1 = 0,
            Perspective2 = 0,
            Perspective3 = 1
        };
    }
    
    public static SkCanvasMatrix CreateScale(float x, float y)
    {
        return new SkCanvasMatrix
        {
            ScaleX = x,
            SkewX = 0,
            TranslateX = 0,
            
            SkewY = 0,
            ScaleY = y,
            TranslateY = 0,
            
            Perspective1 = 0,
            Perspective2 = 0,
            Perspective3 = 1
        };
    }
    
    public static SkCanvasMatrix CreateRotation(float angle)
    {
        var radians = Math.PI * angle / 180;
        var cos = (float)Math.Cos(radians);
        var sin = (float)Math.Sin(radians);
        
        return new SkCanvasMatrix
        {
            ScaleX = cos,
            SkewX = sin,
            TranslateX = 0,
            
            SkewY = -sin,
            ScaleY = cos,
            TranslateY = 0,
            
            Perspective1 = 0,
            Perspective2 = 0,
            Perspective3 = 1
        };
    }
    
    public static SkCanvasMatrix operator *(SkCanvasMatrix a, SkCanvasMatrix b)
    {
        return new SkCanvasMatrix
        {
            ScaleX = a.ScaleX * b.ScaleX + a.SkewY * b.SkewX,
            SkewX = a.ScaleX * b.SkewY + a.SkewY * b.ScaleY,
            TranslateX = a.ScaleX * b.TranslateX + a.SkewY * b.TranslateY + a.TranslateX,
            
            SkewY = a.SkewX * b.ScaleX + a.ScaleY * b.SkewX,
            ScaleY = a.SkewX * b.SkewY + a.ScaleY * b.ScaleY,
            TranslateY = a.SkewX * b.TranslateX + a.ScaleY * b.TranslateY + a.TranslateY,
            
            Perspective1 = 0,
            Perspective2 = 0,
            Perspective3 = 1
        };
    }
}