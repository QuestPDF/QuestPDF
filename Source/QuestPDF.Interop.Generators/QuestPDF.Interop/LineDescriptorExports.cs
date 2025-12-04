using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Interop;

public unsafe partial class Exports
{
    [UnmanagedCallersOnly(EntryPoint = "questpdf__line_descriptor__line_dash_pattern", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static IntPtr LineDescriptor_LineDashPattern(IntPtr target, float* values, int valuesLength, int unit)
    {
        var targetObject = UnboxHandle<LineDescriptor>(target);

        var array = new float[valuesLength];
        new Span<float>(values, valuesLength).CopyTo(array);
    
        var result = targetObject.LineDashPattern(array, (QuestPDF.Infrastructure.Unit)unit);
        return BoxHandle(result);
    }
    
    [UnmanagedCallersOnly(EntryPoint = "questpdf__line_descriptor__line_gradient", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static IntPtr LineDescriptor_LineGradient(IntPtr target, uint* colors, int colorsLength)
    {
        var targetObject = UnboxHandle<LineDescriptor>(target);
    
        var colorArray = new Color[colorsLength];
        
        for (var i = 0; i < colorsLength; i++)
            colorArray[i] = colors[i];
        
        var result = targetObject.LineGradient(colorArray);
        return BoxHandle(result);
    }
}