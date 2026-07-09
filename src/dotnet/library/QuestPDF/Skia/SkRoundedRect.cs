using System.Runtime.InteropServices;

namespace QuestPDF.Skia;

[StructLayout(LayoutKind.Sequential)]
internal struct SkRoundedRect
{
    public SkRect Rect;
    public SkPoint TopLeftRadius;
    public SkPoint TopRightRadius;
    public SkPoint BottomRightRadius;
    public SkPoint BottomLeftRadius;
}