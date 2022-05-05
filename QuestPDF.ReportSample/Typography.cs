using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample
{
    public static class Typography
    {
        public static TextStyle Title => TextStyle.Default.FontFamily(Fonts.Calibri).FontColor(Colors.Blue.Darken3).FontSize(26).Black();
        public static TextStyle Headline => TextStyle.Default.FontFamily(Fonts.Calibri).FontColor(Colors.Blue.Medium).FontSize(16).SemiBold();
        public static TextStyle Normal => TextStyle.Default.FontFamily(Fonts.Verdana).FontColor(Colors.Black).FontSize(10).LineHeight(1.2f);
    }
}
