using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using QuestPDF.Infrastructure;

namespace QuestPDF.Interop;

public unsafe partial class Exports
{
    [UnmanagedCallersOnly(EntryPoint = "questpdf__unit__convert_to_points", CallConvs = new[] { typeof(CallConvCdecl) })]
    public static float Settings_CheckIfAllTextGlyphsAreAvailable(float value, int unit)
    {
        return value.ToPoints((QuestPDF.Infrastructure.Unit)unit);
    }
}