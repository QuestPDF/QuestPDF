using System;
using QuestPDF.Infrastructure;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace QuestPDF.Helpers
{
    /// <summary>
    /// Defines the physical dimensions (width and height) of a page.
    /// </summary>
    /// <remarks>
    /// <para>Commonly used page sizes are available in the <see cref="PageSizes"/> class.</para>
    /// <para>Change page orientation with the <see cref="PageSizeExtensions.Portrait">Portrait</see> and <see cref="PageSizeExtensions.Landscape">Landscape</see> extension methods.</para>
    /// </remarks>
    /// <example>
    /// <c>PageSizes.A4.Landscape();</c>
    /// </example>
    public sealed class PageSize
    {
        public readonly float Width;
        public readonly float Height;
        
        public PageSize(float width, float height, Unit unit = Unit.Point)
        {
            Width = width.ToPoints(unit);
            Height = height.ToPoints(unit);
        }

        public static implicit operator Size(PageSize pageSize) => new Size(pageSize.Width, pageSize.Height);
    }

    /// <summary>
    /// Contains a collection of predefined, common and standard page sizes, such as A4 with dimensions of 595 points in width and 842 points in height.
    /// </summary>
    public static class PageSizes
    {
        public const int PointsPerInch = 72;
        
        public static PageSize A0 => new PageSize(2384, 3370);
        public static PageSize A1 => new PageSize(1684, 2384);
        public static PageSize A2 => new PageSize(1191, 1684);
        public static PageSize A3 => new PageSize(842, 1191);
        public static PageSize A4 => new PageSize(595, 842);
        public static PageSize A5 => new PageSize(420, 595);
        public static PageSize A6 => new PageSize(298, 420);
        public static PageSize A7 => new PageSize(210, 298);
        public static PageSize A8 => new PageSize(147, 210);
        public static PageSize A9 => new PageSize(105, 147);
        public static PageSize A10 => new PageSize(74, 105);

        public static PageSize B0 => new PageSize(2835, 4008);
        public static PageSize B1 => new PageSize(2004, 2835);
        public static PageSize B2 => new PageSize(1417, 2004);
        public static PageSize B3 => new PageSize(1001, 1417);
        public static PageSize B4 => new PageSize(709, 1001);
        public static PageSize B5 => new PageSize(499, 709);
        public static PageSize B6 => new PageSize(354, 499);
        public static PageSize B7 => new PageSize(249, 354);
        public static PageSize B8 => new PageSize(176, 249);
        public static PageSize B9 => new PageSize(125, 176);
        public static PageSize B10 => new PageSize(88, 125);

        public static PageSize C0 => new PageSize(2599, 3677);
        public static PageSize C1 => new PageSize(1837, 2599);
        public static PageSize C2 => new PageSize(1298, 1837);
        public static PageSize C3 => new PageSize(918, 1298);
        public static PageSize C4 => new PageSize(649, 918);
        public static PageSize C5 => new PageSize(459, 649);
        public static PageSize C6 => new PageSize(323, 459);
        public static PageSize C7 => new PageSize(230, 323);
        public static PageSize C8 => new PageSize(162, 230);
        public static PageSize C9 => new PageSize(113, 162);
        public static PageSize C10 => new PageSize(79, 113);

        public static PageSize Env10 => new PageSize(297, 684);
        public static PageSize EnvC4 => new PageSize(649, 918);
        public static PageSize EnvDL => new PageSize(312, 624);

        public static PageSize Postcard => new PageSize(284, 419);
        public static PageSize Executive => new PageSize(522, 756);
        public static PageSize Letter => new PageSize(612, 792);
        public static PageSize Legal => new PageSize(612, 1008);
        public static PageSize Ledger => new PageSize(792, 1224);
        public static PageSize Tabloid => new PageSize(1224, 792);

        public static PageSize ARCH_A => new PageSize(648, 864);
        public static PageSize ARCH_B => new PageSize(864, 1296);
        public static PageSize ARCH_C => new PageSize(1296, 1728);
        public static PageSize ARCH_D => new PageSize(1728, 2592);
        public static PageSize ARCH_E => new PageSize(2592, 3456);
        public static PageSize ARCH_E1 => new PageSize(2160, 3024);
        public static PageSize ARCH_E2 => new PageSize(1872, 2736);
        public static PageSize ARCH_E3 => new PageSize(1944, 2808);
    }

    public static class PageSizeExtensions
    {
        /// <summary>
        /// Sets page size to a portrait orientation, making the width smaller than the height.
        /// </summary>
        public static PageSize Portrait(this PageSize size)
        {
            return new PageSize(Math.Min(size.Width, size.Height), Math.Max(size.Width, size.Height));
        }

        /// <summary>
        /// Sets page size to a landscape orientation, making the width bigger than the height.
        /// </summary>
        public static PageSize Landscape(this PageSize size)
        {
            return new PageSize(Math.Max(size.Width, size.Height), Math.Min(size.Width, size.Height));
        }
    }
}
