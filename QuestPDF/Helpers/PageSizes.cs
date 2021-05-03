using System;
using QuestPDF.Infrastructure;

namespace QuestPDF.Helpers
{
    public class PageSize : Size
    {
        public PageSize(float width, float height) : base(width, height)
        {

        }
    }

    public static class PageSizes
    {
        public const int PointsPerInch = 72;

        public static Size A0 => new PageSize(2384.2f, 3370.8f);
        public static Size A1 => new PageSize(1684, 2384.2f);
        public static Size A2 => new PageSize(1190.7f, 1684);
        public static Size A3 => new PageSize(842, 1190.7f);
        public static Size A4 => new PageSize(595.4f, 842);
        public static Size A5 => new PageSize(419.6f, 595.4f);
        public static Size A6 => new PageSize(297.7f, 419.6f);
        public static Size A7 => new PageSize(209.8f, 297.7f);
        public static Size A8 => new PageSize(147.4f, 209.8f);
        public static Size A9 => new PageSize(104.9f, 147.4f);
        public static Size A10 => new PageSize(73.7f, 104.9f);

        public static Size B0 => new PageSize(2835, 4008.7f);
        public static Size B1 => new PageSize(2004.3f, 2835);
        public static Size B2 => new PageSize(1417.5f, 2004.3f);
        public static Size B3 => new PageSize(1000.8f, 1417.5f);
        public static Size B4 => new PageSize(708.8f, 1000.8f);
        public static Size B5 => new PageSize(499, 708.8f);
        public static Size B6 => new PageSize(354.4f, 499);
        public static Size B7 => new PageSize(249.5f, 354.4f);
        public static Size B8 => new PageSize(175.8f, 249.5f);
        public static Size B9 => new PageSize(124.7f, 175.8f);
        public static Size B10 => new PageSize(87.9f, 124.7f);

        public static Size Env10 => new PageSize(683.2f, 294.8f);
        public static Size EnvC4 => new PageSize(649.2f, 918.5f);
        public static Size EnvDL => new PageSize(311.9f, 623.7f);

        public static Size Executive => new PageSize(522, 756);
        public static Size Legal => new PageSize(612.4f, 1009.3f);
        public static Size Letter => new PageSize(612.4f, 791);

        public static Size ARCH_A => new PageSize(649.2f, 864.7f);
        public static Size ARCH_B => new PageSize(864.7f, 1295.6f);
        public static Size ARCH_C => new PageSize(1295.6f, 1729.3f);
        public static Size ARCH_D => new PageSize(1729.3f, 2591.2f);
        public static Size ARCH_E => new PageSize(2591.2f, 3455.9f);
        public static Size ARCH_E1 => new PageSize(2160.3f, 3024.9f);
        public static Size ARCH_E2 => new PageSize(1871.1f, 2735.8f);
        public static Size ARCH_E3 => new PageSize(1944.8f, 2809.5f);
    }

    public static class PageSizeExtensions
    {
        public static PageSize Portrait(this Size size)
        {
            return new PageSize(Math.Min(size.Width, size.Height), Math.Max(size.Width, size.Height));
        }

        public static PageSize Landscape(this Size size)
        {
            return new PageSize(Math.Max(size.Width, size.Height), Math.Min(size.Width, size.Height));
        }
    }
}
