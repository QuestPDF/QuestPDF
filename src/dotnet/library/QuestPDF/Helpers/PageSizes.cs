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
            if (width < 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Page width must be greater than 0.");
            
            if (height < 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Page height must be greater than 0.");
            
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
        
        public static PageSize A0 { get; } = new(2384, 3370);
        public static PageSize A1 { get; } = new(1684, 2384);
        public static PageSize A2 { get; } = new(1191, 1684);
        public static PageSize A3 { get; } = new(842, 1191);
        public static PageSize A4 { get; } = new(595, 842);
        public static PageSize A5 { get; } = new(420, 595);
        public static PageSize A6 { get; } = new(298, 420);
        public static PageSize A7 { get; } = new(210, 298);
        public static PageSize A8 { get; } = new(147, 210);
        public static PageSize A9 { get; } = new(105, 147);
        public static PageSize A10 { get; } = new(74, 105);

        public static PageSize B0 { get; } = new(2835, 4008);
        public static PageSize B1 { get; } = new(2004, 2835);
        public static PageSize B2 { get; } = new(1417, 2004);
        public static PageSize B3 { get; } = new(1001, 1417);
        public static PageSize B4 { get; } = new(709, 1001);
        public static PageSize B5 { get; } = new(499, 709);
        public static PageSize B6 { get; } = new(354, 499);
        public static PageSize B7 { get; } = new(249, 354);
        public static PageSize B8 { get; } = new(176, 249);
        public static PageSize B9 { get; } = new(125, 176);
        public static PageSize B10 { get; } = new(88, 125);

        public static PageSize C0 { get; } = new(2599, 3677);
        public static PageSize C1 { get; } = new(1837, 2599);
        public static PageSize C2 { get; } = new(1298, 1837);
        public static PageSize C3 { get; } = new(918, 1298);
        public static PageSize C4 { get; } = new(649, 918);
        public static PageSize C5 { get; } = new(459, 649);
        public static PageSize C6 { get; } = new(323, 459);
        public static PageSize C7 { get; } = new(230, 323);
        public static PageSize C8 { get; } = new(162, 230);
        public static PageSize C9 { get; } = new(113, 162);
        public static PageSize C10 { get; } = new(79, 113);

        public static PageSize Env10 { get; } = new(297, 684);
        public static PageSize EnvC4 { get; } = new(649, 918);
        public static PageSize EnvDL { get; } = new(312, 624);

        public static PageSize Postcard { get; } = new(284, 419);
        public static PageSize Executive { get; } = new(522, 756);
        public static PageSize Letter { get; } = new(612, 792);
        public static PageSize Legal { get; } = new(612, 1008);
        public static PageSize Ledger { get; } = new(792, 1224);
        public static PageSize Tabloid { get; } = new(1224, 792);

        public static PageSize ARCH_A { get; } = new(648, 864);
        public static PageSize ARCH_B { get; } = new(864, 1296);
        public static PageSize ARCH_C { get; } = new(1296, 1728);
        public static PageSize ARCH_D { get; } = new(1728, 2592);
        public static PageSize ARCH_E { get; } = new(2592, 3456);
        public static PageSize ARCH_E1 { get; } = new(2160, 3024);
        public static PageSize ARCH_E2 { get; } = new(1872, 2736);
        public static PageSize ARCH_E3 { get; } = new(1944, 2808);
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
