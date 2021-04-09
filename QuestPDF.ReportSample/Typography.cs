using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ReportSample
{
    public static class Typography
    {
        public static TextStyle Title => TextStyle.Default.FontType("Helvetica").Color(Colors.Black).Size(22).SemiBold();
        public static TextStyle Headline => TextStyle.Default.FontType("Helvetica").Color(Colors.Blue.Accent2).Size(14).SemiBold();
        public static TextStyle Normal => TextStyle.Default.FontType("Helvetica").Color(Colors.Black).Size(10).LineHeight(1.25f).AlignLeft();
    }
}