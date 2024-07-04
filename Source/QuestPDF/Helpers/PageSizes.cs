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
    public class PageSize
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
    /// Contains a collection of predefined, common and standard page sizes, such as A4 with dimensions of 595.4 inches in width and 842 inches in height.
    /// </summary>
    public static class PageSizes
    {
        public const int PointsPerInch = 72;

        public static PageSize A0 => new PageSize(2384.2f, 3370.8f);
        public static PageSize A1 => new PageSize(1684, 2384.2f);
        public static PageSize A2 => new PageSize(1190.7f, 1684);
        public static PageSize A3 => new PageSize(842, 1190.7f);
        public static PageSize A4 => new PageSize(595.4f, 842);
        public static PageSize A5 => new PageSize(419.6f, 595.4f);
        public static PageSize A6 => new PageSize(297.7f, 419.6f);
        public static PageSize A7 => new PageSize(209.8f, 297.7f);
        public static PageSize A8 => new PageSize(147.4f, 209.8f);
        public static PageSize A9 => new PageSize(104.9f, 147.4f);
        public static PageSize A10 => new PageSize(73.7f, 104.9f);

        public static PageSize B0 => new PageSize(2835, 4008.7f);
        public static PageSize B1 => new PageSize(2004.3f, 2835);
        public static PageSize B2 => new PageSize(1417.5f, 2004.3f);
        public static PageSize B3 => new PageSize(1000.8f, 1417.5f);
        public static PageSize B4 => new PageSize(708.8f, 1000.8f);
        public static PageSize B5 => new PageSize(499, 708.8f);
        public static PageSize B6 => new PageSize(354.4f, 499);
        public static PageSize B7 => new PageSize(249.5f, 354.4f);
        public static PageSize B8 => new PageSize(175.8f, 249.5f);
        public static PageSize B9 => new PageSize(124.7f, 175.8f);
        public static PageSize B10 => new PageSize(87.9f, 124.7f);

        public static PageSize Env10 => new PageSize(683.2f, 294.8f);
        public static PageSize EnvC4 => new PageSize(649.2f, 918.5f);
        public static PageSize EnvDL => new PageSize(311.9f, 623.7f);

        public static PageSize Executive => new PageSize(522, 756);
        public static PageSize Legal => new PageSize(612, 1008);
        public static PageSize Letter => new PageSize(612, 792);

        public static PageSize ARCH_A => new PageSize(649.2f, 864.7f);
        public static PageSize ARCH_B => new PageSize(864.7f, 1295.6f);
        public static PageSize ARCH_C => new PageSize(1295.6f, 1729.3f);
        public static PageSize ARCH_D => new PageSize(1729.3f, 2591.2f);
        public static PageSize ARCH_E => new PageSize(2591.2f, 3455.9f);
        public static PageSize ARCH_E1 => new PageSize(2160.3f, 3024.9f);
        public static PageSize ARCH_E2 => new PageSize(1871.1f, 2735.8f);
        public static PageSize ARCH_E3 => new PageSize(1944.8f, 2809.5f);

        public static bool TryParsePageSize(string size, out PageSize pageSize)
        {
            switch (size?.ToLower())
            {
                case "a0": pageSize = A0; break;
                case "a1": pageSize = A1; break;
                case "a2": pageSize = A2; break;
                case "a3": pageSize = A3; break;
                case "a4": pageSize = A4; break;
                case "a5": pageSize = A5; break;
                case "a6": pageSize = A6; break;
                case "a7": pageSize = A7; break;
                case "a8": pageSize = A8; break;
                case "a9": pageSize = A9; break;
                case "a10": pageSize = A10; break;
                case "b0": pageSize = B0; break;
                case "b1": pageSize = B1; break;
                case "b2": pageSize = B2; break;
                case "b3": pageSize = B3; break;
                case "b4": pageSize = B4; break;
                case "b5": pageSize = B5; break;
                case "b6": pageSize = B6; break;
                case "b7": pageSize = B7; break;
                case "b8": pageSize = B8; break;
                case "b9": pageSize = B9; break;
                case "b10": pageSize = B10; break;
                case "env10": pageSize = Env10; break;
                case "envc4": pageSize = EnvC4; break;
                case "envdl": pageSize = EnvDL; break;
                case "executive": pageSize = Executive; break;
                case "legal": pageSize = Legal; break;
                case "letter": pageSize = Letter; break;
                case "arch_a": pageSize = ARCH_A; break;
                case "arch_b": pageSize = ARCH_B; break;
                case "arch_c": pageSize = ARCH_C; break;
                case "arch_d": pageSize = ARCH_D; break;
                case "arch_e": pageSize = ARCH_E; break;
                case "arch_e1": pageSize = ARCH_E1; break;
                case "arch_e2": pageSize = ARCH_E2; break;
                case "arch_e3": pageSize = ARCH_E3; break;
                default:
                    pageSize = null;
                    return false;
            }
            return true;
        }

        public static PageSize ParsePageSize(string size)
        {
            if(!TryParsePageSize(size, out PageSize pageSize))
                throw new ArgumentException("Invalid page size");

            return pageSize;
        }
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
