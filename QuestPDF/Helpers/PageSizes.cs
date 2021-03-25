using QuestPDF.Infrastructure;

namespace QuestPDF.Helpers
{
    public static class PageSizes
    {
        public const int PointsPerInch = 72;

        public static Size A0 => new Size(2384, 3370);
        public static Size A1 => new Size(1684, 2384);
        public static Size A2 => new Size(1191, 1684);
        public static Size A3 => new Size(842, 1191);
        public static Size A4 => new Size(595, 842);
        public static Size A5 => new Size(420, 595);
        public static Size A6 => new Size(297, 420);
        public static Size A7 => new Size(210, 297);
        public static Size A8 => new Size(148, 210);
        public static Size A9 => new Size(105, 148);
        public static Size A10 => new Size(73, 105);

        public static Size B0 => new Size(2920, 4127);
        public static Size B1 => new Size(2064, 2920);
        public static Size B2 => new Size(1460, 2064);
        public static Size B3 => new Size(1032, 1460);
        public static Size B4 => new Size(729, 1032);
        public static Size B5 => new Size(516, 729);
        public static Size B6 => new Size(363, 516);
        public static Size B7 => new Size(258, 363);
        public static Size B8 => new Size(181, 258);
        public static Size B9 => new Size(127, 181);
        public static Size B10 => new Size(91, 127);

        public static Size Env10 => new Size(297, 684);
        public static Size EnvC4 => new Size(649, 918);

        public static Size Executive => new Size(522, 756);
        public static Size Legal => new Size(612, 1008);
        public static Size Letter => new Size(612, 792);
        public static Size Postcard => new Size(284, 419);

        public static Size ARCHA => new Size(648, 864);
        public static Size ARCHB => new Size(864, 1296);
        public static Size ARCHC => new Size(1296, 1728);
        public static Size ARCHD => new Size(1728, 2592);
        public static Size ARCHE => new Size(2592, 3456);
        public static Size ARCH_E1 => new Size(2160, 3025);
        public static Size ARCH_E2 => new Size(1871, 2735);
        public static Size ARCH_E3 => new Size(1945, 2809);
    }
}
