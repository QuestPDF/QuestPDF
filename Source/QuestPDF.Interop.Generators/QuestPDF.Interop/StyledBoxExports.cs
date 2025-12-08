using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Interop;

public unsafe partial class Exports
{
    [UnmanagedCallersOnly(EntryPoint = "questpdf__container__background_linear_gradient", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static IntPtr Container_BackgroundLinearGradient(IntPtr target, float angle, uint* colors, int colorsLength)
    {
        var targetObject = UnboxHandle<IContainer>(target);
        FreeHandle(target);

        var colorArray = new Color[colorsLength];
        
        for (var i = 0; i < colorsLength; i++)
            colorArray[i] = colors[i];
    
        var result = targetObject.BackgroundLinearGradient(angle, colorArray);
        return BoxHandle(result);
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__container__border_linear_gradient", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static IntPtr Container_BorderLinearGradient(IntPtr target, float angle, uint* colors, int colorsLength)
    {
        var targetObject = UnboxHandle<IContainer>(target);
        FreeHandle(target);
    
        var colorArray = new Color[colorsLength];
        
        for (var i = 0; i < colorsLength; i++)
            colorArray[i] = colors[i];
        
        var result = targetObject.BorderLinearGradient(angle, colorArray);
        return BoxHandle(result);
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__container__shadow", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static IntPtr Container_Shadow(IntPtr target, float blur, uint color, float offsetX, float offsetY, float spread)
    {
        var targetObject = UnboxHandle<IContainer>(target);
        FreeHandle(target);
        
        var result = targetObject.Shadow(new BoxShadowStyle()
        {
            Blur = blur,
            Color = color,
            OffsetX = offsetX,
            OffsetY = offsetY,
            Spread = spread
        });
        
        return BoxHandle(result);
    }
}