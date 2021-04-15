using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample
{
    public static class Typography
    {
        public static TextStyle Title => TextStyle.Default.FontType(Fonts.Calibri).Color(Colors.Black).Size(24).Black();
        public static TextStyle Headline => TextStyle.Default.FontType(Fonts.Calibri).Color(Colors.Blue.Medium).Size(16).SemiBold();
        public static TextStyle Normal => TextStyle.Default.FontType(Fonts.Calibri).Color(Colors.Black).Size(11).LineHeight(1.25f).AlignLeft();
    }
}