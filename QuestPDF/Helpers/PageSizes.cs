using QuestPDF.Infrastructure;

namespace QuestPDF.Helpers
{
    public static class PageSizes
    {
        public static Size A0 => new Size(2384, 3370);
        public static Size A1 => new Size(1684, 2384);
        public static Size A2 => new Size(1190, 1684);
        public static Size A3 => new Size(842, 1190);
        public static Size A4 => new Size(595, 842);
        public static Size A5 => new Size(420, 595);
        public static Size A6 => new Size(298, 420);
        public static Size A7 => new Size(210, 298);
        public static Size A8 => new Size(148, 210);
        
        public static Size B0 => new Size(2835, 4008);
        public static Size B1 => new Size(2004, 2835);
        public static Size B2 => new Size(1417, 2004);
        public static Size B3 => new Size(1001, 1417);
        public static Size B4 => new Size(709, 1001);
        public static Size B5 => new Size(499, 709);
        public static Size B6 => new Size(354, 499);
        public static Size B7 => new Size(249, 354);
        public static Size B8 => new Size(176, 249);
        public static Size B9 => new Size(125, 176);
        public static Size B10 => new Size(88, 125);
    }
}
